using Microsoft.EntityFrameworkCore;
using shrash_tech_certificate_gen_api.Entities;

namespace shrash_tech_certificate_gen_api.AppData
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Certificates> Certificates { get; set; }
		public DbSet<EmailStatus> EmailStatus { get; set; }
		public DbSet<ContactUs> ContactUs { get; set; }
	}
}
