using Microsoft.EntityFrameworkCore;
using Serilog;
using shrash_tech_certificate_gen_api.AppData;

var logDir = Path.Combine(AppContext.BaseDirectory, "log");
Directory.CreateDirectory(logDir);
var logPath = Path.Combine(logDir, "log-.txt");

Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine("SERILOG ERROR: " + msg));

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()
	.WriteTo.Console()
	.WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
	.CreateLogger();

try
{
	Log.Information("Starting application");

	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseSerilog();

	// 🔥 This is critical to make ILogger<> work with Serilog
	builder.Logging.ClearProviders();
	builder.Logging.AddSerilog();

	builder.Services.AddDbContext<AppDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

	builder.Services.AddTransient<EmailService>();
	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	var app = builder.Build();
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseHttpsRedirection();
	app.UseAuthorization();
	app.MapControllers();

	Log.Information("App is up and running.");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application start-up failed");
}
finally
{
	Log.CloseAndFlush();
}
