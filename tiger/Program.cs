using Serilog;
using tiger;
using Microsoft.Extensions.Hosting.WindowsServices;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("C:/ProgramData/YourServiceName/logs.txt", rollingInterval: RollingInterval.Day) // Add this line to write logs to a file
    .CreateLogger();

try
{
    Log.Information("Starting up");

    IHost host = Host.CreateDefaultBuilder(args)
        .UseSerilog() // Add this line to use Serilog
        .UseWindowsService() // Add this line to integrate with Windows Service
        .ConfigureServices(services =>
        {
            services.AddHostedService<Worker>();
        })
        .Build();

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}