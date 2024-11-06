using Serilog;
using tiger;
using System.Net.Mail;
using System.Runtime.InteropServices;
using tiger.helpers;

// Some Logic to change paths depending on the OS 
string writeToLocation = PlatformConfig.GetWriteToLocation();
string tigerPath = PlatformConfig.GetTigerPath();

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(writeToLocation, rollingInterval: RollingInterval.Day) // Add this line to write logs to a file
    .CreateLogger();

try
{
    Log.Information("Starting up");

    // "App" builder
    IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
        .UseSerilog() // Add this line to use Serilog
        .ConfigureServices(services =>
        {
    	    services.AddSingleton(args);
            services.AddHostedService<Worker>();
        });

    // Check OS
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        hostBuilder = hostBuilder.UseWindowsService();
        Log.Information("Running as Windows Service");
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        hostBuilder = hostBuilder.UseSystemd();
        Log.Information("Running as Linux Daemon");
    }

    IHost host = hostBuilder.Build();
    string tiger_ascii = File.ReadAllText(tigerPath + "tiger-ascii.txt");
    Log.Information(tiger_ascii);
    Log.Information("tiger-daemon initialized");
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
