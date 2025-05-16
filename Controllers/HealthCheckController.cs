using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace shrash_tech_certificate_gen_api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HealthCheckController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok("Welcome to Shrash Tech Certificate Generation API");
		}
	}
}
