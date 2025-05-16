using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace shrash_tech_certificate_gen_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LogsController : ControllerBase
	{
		private readonly string _logFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "log");

		// GET: api/logs/today
		[HttpGet("today")]
		public async Task<IActionResult> GetTodayLog()
		{
			string fileName = $"log-{DateTime.UtcNow:yyyyMMdd}.txt";
			string fullPath = Path.Combine(_logFolderPath, fileName);

			if (!System.IO.File.Exists(fullPath))
				return NotFound(new { message = "Today's log file not found." });

			var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
			return File(bytes, "text/plain", fileName);
		}

		// GET: api/logs/20250514
		[HttpGet("{date}")]
		public async Task<IActionResult> GetLogByDate(string date)
		{
			if (!DateTime.TryParseExact(date, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
				return BadRequest(new { message = "Invalid date format. Use yyyyMMdd." });

			string fileName = $"log-{parsedDate:yyyyMMdd}.txt";
			string fullPath = Path.Combine(_logFolderPath, fileName);

			if (!System.IO.File.Exists(fullPath))
				return NotFound(new { message = "Log file for the given date not found." });

			var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
			return File(bytes, "text/plain", fileName);
		}
	}
}
