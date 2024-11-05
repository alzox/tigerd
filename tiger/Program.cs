using Serilog;
using tiger;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Net.Mail;

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("C:/ProgramData/tiger-daemon/logs.txt", rollingInterval: RollingInterval.Day) // Add this line to write logs to a file
    .CreateLogger();

try
{
    Log.Information("Starting up");

    // Window SCM service
    string contextPath = AppContext.BaseDirectory;
    int index = contextPath.IndexOf("tiger\\");
    string tigerPath = contextPath.Substring(0, index + 6);
    var configuration = new ConfigurationBuilder()
    .SetBasePath(tigerPath)
    .AddJsonFile("config", optional: false, reloadOnChange: true)
    .Build();

    // SMTP settings
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
    IHost host = Host.CreateDefaultBuilder(args)
        .UseSerilog() // Add this line to use Serilog
        .UseWindowsService() // Add this line to integrate with Windows Service
        .ConfigureServices(services =>
        {
            services.AddSingleton(smtpClient);
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