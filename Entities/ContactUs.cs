using System;
using System.ComponentModel.DataAnnotations;

namespace shrash_tech_certificate_gen_api.Entities
{
	public class ContactUs
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[MaxLength(100)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[MaxLength(15)]
		public string Phone { get; set; } = string.Empty;

		[Required]
		[MaxLength(500)]
		public string Message { get; set; } = string.Empty;

		public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
	}
}
