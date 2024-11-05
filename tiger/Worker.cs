using System.Diagnostics;
using System.Net.Mail;
using tiger.helpers;

namespace tiger;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly SmtpClient _smtpClient;
    private readonly string[] _args;

    public Worker(ILogger<Worker> logger, SmtpClient smtpClient, string[] args)
    {
        _logger = logger;
        _smtpClient = smtpClient;
	    _args = args;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
	foreach (string arg in _args){
		_logger.LogInformation(arg);
	}
        ProccessDict proccessDict = new ProccessDict(_logger);
        proccessDict.Initialize();

	if (_args.Length > 0) {
		MessageJson messages = new MessageJson(_logger, _args[0]);
	}
        while (!stoppingToken.IsCancellationRequested)
        {
            proccessDict.Update();
            CheckProcessesPlaytime(proccessDict.GetDict());
            await Task.Delay(1800000, stoppingToken);
        }
    }

    public void CheckProcessesPlaytime(Dictionary<string, TimeSpan> processesDict)
    {
        foreach (KeyValuePair<string, TimeSpan> entry in processesDict)
        {
            string processName = entry.Key;
            TimeSpan timeElapsed = entry.Value;
            if (timeElapsed.TotalMinutes > 1)
            {
                string subject = "wee woo wee woo";
		string body = "hey, get off " + processName;
                string to = "djx3rn@virginia.edu";
                SendEmail(subject, body, to);
            }
        }
    }
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async void SendEmail(string subject, string body, string to)
    {
        await _semaphore.WaitAsync();
        try
        {
            var fromMailAddress = new MailAddress("dependsonthesmtp@gmail.com", "tiger"); // some smtp overrides this but gmail is chill
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
        finally
        {
            _semaphore.Release();
        }
    }
}
