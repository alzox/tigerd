using Serilog;
using tiger;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Net.Mail;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("C:/ProgramData/tiger-daemon/logs.txt", rollingInterval: RollingInterval.Day) // Add this line to write logs to a file
    .CreateLogger();

try
{
    Log.Information("Starting up");
    Log.Information($"AppContext.BaseDirectory: {AppContext.BaseDirectory}");

    // Load configuration from config file
    var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("C:/Users/commo/OneDrive - University of Virginia/School/STEM/CS/Coding Projects 2024/tiger-daemon/tiger/config", optional: false, reloadOnChange: true)
    .Build();

    IConfigurationSection smtpSettings = configuration.GetSection("SmtpSettings");
    string smtpServer = smtpSettings["Server"];
    int smtpPort = int.Parse(smtpSettings["Port"]);
    string smtpEmail = smtpSettings["Email"];
    string smtpPassword = smtpSettings["Password"];

    // Set up SMTP client from config 
    SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);

    smtpClient.Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPassword);
    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    smtpClient.EnableSsl = true;

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