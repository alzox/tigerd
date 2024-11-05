using Serilog;
using tiger;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Net.Mail;
using System.Runtime.InteropServices;

// Some Logic to change paths depending on the OS 
string writeToLocation = "";
string contextPath = AppContext.BaseDirectory;
int index = contextPath.IndexOf("tiger\\");
 
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
	writeToLocation = "C:/ProgramData/tiger-daemon/logs.txt";
	int index = contextPath.IndexOf("tiger\\");
}
else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux){
	writeToLocation = "/var/log/tiger-daemon/logs.txt";
	int index = contextPath.IndexOf("tiger/");
}

string tigerPath = contextPath.Substring(0, index + 6);

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(writeToLocation, rollingInterval: RollingInterval.Day) // Add this line to write logs to a file
    .CreateLogger();

try
{
    Log.Information("Starting up");

    // SMTP settings
   var configuration = new ConfigurationBuilder()
    .SetBasePath(tigerPath)
    .AddJsonFile("config", optional: false, reloadOnChange: true)
    .Build();
    IConfigurationSection smtpSettings = configuration.GetSection("SmtpSettings");
    string smtpServer = smtpSettings["Server"];
    int smtpPort = int.Parse(smtpSettings["Port"]);
    string smtpEmail = smtpSettings["Email"];
    string smtpPassword = smtpSettings["Password"];
    SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
    smtpClient.Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPassword);
    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    smtpClient.EnableSsl = true;

    // "App" builder
    IHostBuilder hostBuidler = Host.CreateDefaultBuilder(args)
        .UseSerilog() // Add this line to use Serilog
        .ConfigureServices(services =>
        {
            services.AddSingleton(smtpClient);
            services.AddHostedService<Worker>();
        });

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

    IHost host = host.Build();
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
