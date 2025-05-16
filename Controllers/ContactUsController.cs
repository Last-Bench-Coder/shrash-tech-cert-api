using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using shrash_tech_certificate_gen_api.AppData;
using shrash_tech_certificate_gen_api.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shrash_tech_certificate_gen_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContactUsController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly ILogger<ContactUsController> _logger;

		public ContactUsController(AppDbContext context, ILogger<ContactUsController> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: api/ContactUs
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ContactUs>>> GetAllContacts()
		{
			_logger.LogInformation("Fetching all contacts from the database.");
			var contacts = await _context.ContactUs.ToListAsync();
			_logger.LogInformation("Fetched {Count} contacts.", contacts.Count);
			return contacts;
		}

		// GET: api/ContactUs/5
		[HttpGet("{id}")]
		public async Task<ActionResult<ContactUs>> GetContact(int id)
		{
			_logger.LogInformation("Fetching contact with ID: {Id}", id);
			var contact = await _context.ContactUs.FindAsync(id);

			if (contact == null)
			{
				_logger.LogWarning("Contact with ID {Id} not found.", id);
				return NotFound(new { message = "Contact not found." });
			}

			_logger.LogInformation("Contact with ID {Id} retrieved.", id);
			return contact;
		}

		// POST: api/ContactUs
		[HttpPost]
		public async Task<ActionResult<ContactUs>> CreateContact(ContactUs contact)
		{
			contact.SubmittedAt = DateTime.UtcNow;
			_context.ContactUs.Add(contact);
			await _context.SaveChangesAsync();

			_logger.LogInformation("New contact created with ID {Id}", contact.Id);
			return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
		}

		// PUT: api/ContactUs/5
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateContact(int id, ContactUs contact)
		{
			if (id != contact.Id)
			{
				_logger.LogWarning("Update failed: ID mismatch (URL ID: {UrlId}, Body ID: {BodyId})", id, contact.Id);
				return BadRequest(new { message = "ID mismatch" });
			}

			var existing = await _context.ContactUs.FindAsync(id);
			if (existing == null)
			{
				_logger.LogWarning("Contact with ID {Id} not found for update.", id);
				return NotFound(new { message = "Contact not found." });
			}

			existing.Name = contact.Name;
			existing.Email = contact.Email;
			existing.Phone = contact.Phone;
			existing.Message = contact.Message;

			await _context.SaveChangesAsync();
			_logger.LogInformation("Contact with ID {Id} updated.", id);

			return NoContent();
		}

		// DELETE: api/ContactUs/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteContact(int id)
		{
			var contact = await _context.ContactUs.FindAsync(id);
			if (contact == null)
			{
				_logger.LogWarning("Contact with ID {Id} not found for deletion.", id);
				return NotFound(new { message = "Contact not found." });
			}

			_context.ContactUs.Remove(contact);
			await _context.SaveChangesAsync();
			_logger.LogInformation("Contact with ID {Id} deleted.", id);

			return NoContent();
		}
	}
}
