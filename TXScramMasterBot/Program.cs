global using IResult = TXScramMasterBot.Wrapper.IResult;

using Deployf.Botf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using TXScramMasterBot.Context;
using TXScramMasterBot.Contracts;
using TXScramMasterBot.Services;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
if (WindowsServiceHelpers.IsWindowsService())
	Directory.SetCurrentDirectory(AppContext.BaseDirectory);

try
{
	var options = new WebApplicationOptions
	{
		Args = args,
		ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default,
	};
	var builder = WebApplication.CreateBuilder(options);
	builder.Host.UseWindowsService();
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console()
		.ReadFrom.Configuration(builder.Configuration);
	});
	builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));
	builder.Services.AddScoped<IUserService, UserService>();
	builder.Services.AddScoped<ITaskService, TaskService>();
	builder.Services.AddTransient<DbSeeder>();
	builder.Services.AddBotf(builder.Configuration["botf"]);
	builder.Services.AddScoped<IBotUserService, AuthService>();
	var app = builder.Build();
	using (var scope = app.Services.CreateAsyncScope())
	{
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		await context.Database.MigrateAsync();
		await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync();
	}
	app.UseBotf();
	app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
	StaticLogger.EnsureInitialized();
	Log.Fatal(ex, "Unhandled exception");
	Console.ReadKey();
}
finally
{
	StaticLogger.EnsureInitialized();
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}
