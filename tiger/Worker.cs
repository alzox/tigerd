using System.Diagnostics;
using System.Net.Mail;

namespace tiger;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SmtpClient _smtpClient;

    public Worker(ILogger<Worker> logger, SmtpClient smtpClient)
    {
        _logger = logger;
        _smtpClient = smtpClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int count = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            bool isDiscordRunning = Process.GetProcessesByName("Discord").Any();
            await Task.Delay(60000, stoppingToken);                
             if (isDiscordRunning)
            {
                count++;
                _logger.LogInformation($"Discord is running. Count: {count}");
            }
            else
            {
                _logger.LogWarning("Discord is not running.");
            }            
            if (count == 2) {
                SendEmail("Discord is running", "Discord is running on your computer.", "djx3rn@virginia.edu");
                count = 0;
            }

        }
    }

    public async void SendEmail(string subject, string body, string to)
    {
        try
        {
            var fromMailAddress = new MailAddress("henrygao00@gmail.com", "tiger");
            var toMailAddress = new MailAddress(to);
            var mailMessage = new MailMessage(fromMailAddress, toMailAddress)
            {
                Subject = subject,
                Body = body
            };
            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email.");
        }
    }
}