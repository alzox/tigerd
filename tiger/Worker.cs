using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Mail;
using Microsoft.Win32.SafeHandles;

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
            var mailMessage = new MailMessage("henrygao00@gmail.com", to)
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