using System.Net.Mail;

namespace tiger.helpers;

public class SMTPHelper
{
    public SMTPHelper()
    {}

    public SmtpClient GetSmtpClient()
    {
        string tigerPath = PlatformConfig.GetTigerPath();
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
        return smtpClient;
    }
}