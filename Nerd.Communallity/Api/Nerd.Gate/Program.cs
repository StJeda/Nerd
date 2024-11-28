using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
		builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables();
        
		builder.Services.AddOcelot(builder.Configuration);
        builder.Services.AddLogging(x => x.AddConsole());
        
		var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        
		logger.LogInformation("Ocelot API Gateway is starting...");
        
		app.UseOcelot().Wait();
        
		logger.LogInformation("Ocelot API Gateway stopped.");
        
		await app.RunAsync();
    }
}
