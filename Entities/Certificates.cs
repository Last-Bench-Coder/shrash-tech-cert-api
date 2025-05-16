using System.ComponentModel.DataAnnotations;

namespace shrash_tech_certificate_gen_api.Entities
{
	public class Certificates
	{
		[Key]
		public int Id { get; set; }
		public string? CertificateId { get; set; }
		[Required]
		public string? StudentName { get; set; }
		[Required]
		public string? CourseName { get; set; }
		[Required]
		public DateTime Date { get; set; }
		[Required]
		public string? Email { get; set; }
		[Required]
		public string? Phone { get; set; }
		[Required]
		public string? InstituteName { get; set; }
		[Required]
		public string? InstituteAddress { get; set; }
		[Required]
		public string? InstitutePhone { get; set; }
		[Required]
		public string? InstituteEmail { get; set; }
		[Required]
		public string? InstituteLogo { get; set; }
		[Required]
		public string? SignatureName { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
	}
}
