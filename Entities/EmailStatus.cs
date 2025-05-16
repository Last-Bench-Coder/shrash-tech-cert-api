using System;
using System.ComponentModel.DataAnnotations;

namespace shrash_tech_certificate_gen_api.Entities
{
	public class EmailStatus
	{
		[Key]
		public int Id { get; set; }

		// Foreign key to Certificates table
		public int CertificateId { get; set; }

		// Email address to which email was sent
		[Required]
		[EmailAddress]
		public string ToEmail { get; set; } = string.Empty;

		// Status of the email sending attempt (e.g., "Sent", "Failed")
		[Required]
		public string Status { get; set; } = string.Empty;

		// Optional error message if sending failed
		public string? ErrorMessage { get; set; }

		// Timestamp when the email was attempted
		public DateTime SentDate { get; set; } = DateTime.UtcNow;
	}
}
