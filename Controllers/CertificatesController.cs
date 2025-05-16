using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shrash_tech_certificate_gen_api.AppData;
using shrash_tech_certificate_gen_api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shrash_tech_certificate_gen_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CertificatesController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly EmailService _emailService;
		private readonly ILogger<CertificatesController> _logger;

		public CertificatesController(AppDbContext context, EmailService emailService, ILogger<CertificatesController> logger)
		{
			_context = context;
			_emailService = emailService;
			_logger = logger;
		}

		// GET: api/Certificates
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Certificates>>> GetCertificates()
		{
			_logger.LogInformation("Fetching all certificates");
			var certificates = await _context.Certificates.ToListAsync();
			_logger.LogInformation("Fetched {Count} certificates", certificates.Count);
			return certificates;
		}

		// GET: api/Certificates/5
		[HttpGet("{id}")]
		public async Task<ActionResult<Certificates>> GetCertificate(int id)
		{
			_logger.LogInformation("Fetching certificate with Id {Id}", id);
			var certificate = await _context.Certificates.FindAsync(id);

			if (certificate == null)
			{
				_logger.LogWarning("Certificate with Id {Id} not found", id);
				return NotFound(new { message = "Certificate not found or may have been removed." });
			}

			return certificate;
		}

		// GET: api/Certificates/by-certificate-id/CERTST2025110012
		[HttpGet("by-certificate-id/{certificateId}")]
		public async Task<ActionResult<Certificates>> GetCertificateByCertificateId(string certificateId)
		{
			_logger.LogInformation("Fetching certificate with CertificateId {CertificateId}", certificateId);
			var certificate = await _context.Certificates
				.FirstOrDefaultAsync(c => c.CertificateId == certificateId);

			if (certificate == null)
			{
				_logger.LogWarning("Certificate with CertificateId {CertificateId} not found", certificateId);
				return NotFound(new { message = "Certificate not found." });
			}

			return certificate;
		}

		// POST: api/Certificates/{id}/sendemail
		[HttpPost("{id}/sendemail")]
		public async Task<IActionResult> SendCertificateEmail(int id)
		{
			_logger.LogInformation("Sending certificate email for Id {Id}", id);

			var certificate = await _context.Certificates.FindAsync(id);
			if (certificate == null)
			{
				_logger.LogWarning("Certificate with Id {Id} not found when trying to send email", id);
				return NotFound(new { message = "Certificate not found." });
			}

			string downloadUrl = $"https://shrashcert.bsite.net/download/{certificate.CertificateId}";

			var emailStatus = new EmailStatus
			{
				CertificateId = certificate.Id,
				ToEmail = certificate.Email,
				SentDate = DateTime.UtcNow
			};

			try
			{
				await _emailService.SendCertificateEmailAsync(certificate.Email, downloadUrl, certificate);

				emailStatus.Status = "Sent";
				emailStatus.ErrorMessage = null;

				_logger.LogInformation("Email sent successfully to {Email} for certificate Id {Id}", certificate.Email, id);
			}
			catch (Exception ex)
			{
				emailStatus.Status = "Failed";
				emailStatus.ErrorMessage = ex.Message;

				_logger.LogError(ex, "Failed to send email for certificate Id {Id} to {Email}", id, certificate.Email);
			}

			_context.EmailStatus.Add(emailStatus);
			await _context.SaveChangesAsync();

			if (emailStatus.Status == "Sent")
				return Ok(new { message = "Certificate email sent successfully." });
			else
				return StatusCode(500, new { message = "Failed to send email.", error = emailStatus.ErrorMessage });
		}

		// POST: api/Certificates
		[HttpPost]
		public async Task<ActionResult<Certificates>> CreateCertificate(Certificates certificate)
		{
			_logger.LogInformation("Creating new certificate for student {StudentName}", certificate.StudentName);

			certificate.CreatedDate = DateTime.UtcNow;
			certificate.UpdatedDate = DateTime.UtcNow;

			try
			{
				// Step 1: Add certificate to get the auto-incremented Id
				_context.Certificates.Add(certificate);
				await _context.SaveChangesAsync(); // certificate.Id populated

				// Step 2: Generate CertificateId
				string year = DateTime.UtcNow.Year.ToString();
				certificate.CertificateId = $"CERTST{year}11{certificate.Id:D4}";

				// Step 3: Save the CertificateId
				_context.Entry(certificate).Property(c => c.CertificateId).IsModified = true;
				await _context.SaveChangesAsync();

				_logger.LogInformation("Created certificate with Id {Id} and CertificateId {CertificateId}", certificate.Id, certificate.CertificateId);

				return CreatedAtAction(nameof(GetCertificate), new { id = certificate.Id }, certificate);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating certificate for student {StudentName}", certificate.StudentName);
				return StatusCode(500, new { message = "Error creating certificate." });
			}
		}

		// GET: api/Certificates/{id}/emailstatus
		[HttpGet("{id}/emailstatus")]
		public async Task<IActionResult> GetEmailStatusByCertificateId(int id)
		{
			_logger.LogInformation("Fetching email statuses for certificate Id {Id}", id);

			var statuses = await _context.EmailStatus
				.Where(e => e.CertificateId == id)
				.OrderByDescending(e => e.SentDate)
				.ToListAsync();

			if (statuses == null || statuses.Count == 0)
			{
				_logger.LogWarning("No email status found for certificate Id {Id}", id);
				return NotFound(new { message = "No email status found for this certificate." });
			}

			return Ok(statuses);
		}

		// GET: api/EmailStatus
		[HttpGet("/api/EmailStatus")]
		public async Task<IActionResult> GetAllEmailStatuses()
		{
			_logger.LogInformation("Fetching all email statuses");

			var allStatuses = await _context.EmailStatus
				.OrderByDescending(e => e.SentDate)
				.ToListAsync();

			return Ok(allStatuses);
		}

		// PUT: api/Certificates/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCertificate(int id, Certificates certificate)
		{
			_logger.LogInformation("Updating certificate with Id {Id}", id);

			if (id != certificate.Id)
			{
				_logger.LogWarning("ID mismatch on update: route Id {RouteId} != body Id {BodyId}", id, certificate.Id);
				return BadRequest(new { message = "ID mismatch" });
			}

			var existing = await _context.Certificates.FindAsync(id);
			if (existing == null)
			{
				_logger.LogWarning("Certificate with Id {Id} not found for update", id);
				return NotFound(new { message = "Certificate not found." });
			}

			try
			{
				// Update fields
				existing.StudentName = certificate.StudentName;
				existing.CourseName = certificate.CourseName;
				existing.Date = certificate.Date;
				existing.Email = certificate.Email;
				existing.Phone = certificate.Phone;
				existing.InstituteName = certificate.InstituteName;
				existing.InstituteAddress = certificate.InstituteAddress;
				existing.InstitutePhone = certificate.InstitutePhone;
				existing.InstituteEmail = certificate.InstituteEmail;
				existing.InstituteLogo = certificate.InstituteLogo;
				existing.SignatureName = certificate.SignatureName;
				existing.UpdatedDate = DateTime.UtcNow;

				await _context.SaveChangesAsync();

				_logger.LogInformation("Certificate with Id {Id} updated successfully", id);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating certificate with Id {Id}", id);
				return StatusCode(500, new { message = "Error updating certificate." });
			}
		}

		// DELETE: api/Certificates/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCertificate(int id)
		{
			_logger.LogInformation("Deleting certificate with Id {Id}", id);

			var certificate = await _context.Certificates.FindAsync(id);
			if (certificate == null)
			{
				_logger.LogWarning("Certificate with Id {Id} not found for delete", id);
				return NotFound(new { message = "Certificate not found." });
			}

			try
			{
				_context.Certificates.Remove(certificate);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Certificate with Id {Id} deleted successfully", id);
				return Ok(new { message = "Certificate deleted successfully." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting certificate with Id {Id}", id);
				return StatusCode(500, new { message = "Error deleting certificate." });
			}
		}
	}
}
