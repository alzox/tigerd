using Serilog;
using tiger;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Net.Mail;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("C:/ProgramData/YourServiceName/logs.txt", rollingInterval: RollingInterval.Day) // Add this line to write logs to a file
    .CreateLogger();

try
{
    Log.Information("Starting up");

    SmtpClient smtpClient = new SmtpClient("mail.MyWebsiteDomainName.com", 25);

    smtpClient.Credentials = new System.Net.NetworkCredential("info@MyWebsiteDomainName.com", "myIDPassword");
    // smtpClient.UseDefaultCredentials = true; // uncomment if you don't want to use the network credentials
    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    smtpClient.EnableSsl = true;
    MailMessage mail = new MailMessage();

    //Setting From , To and CC
    mail.From = new MailAddress("info@MyWebsiteDomainName", "MyWeb Site");
    mail.To.Add(new MailAddress("info@MyWebsiteDomainName"));
    mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));

    smtpClient.Send(mail);

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