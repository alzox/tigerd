using System.Diagnostics;
using System.Net.Mail;
using tiger.helpers;

namespace tiger;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly string[] _args;

    public Worker(ILogger<Worker> logger, string[] args)
    {
        _logger = logger;
	    _args = args;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
	foreach (string arg in _args){
		_logger.LogInformation(arg);
	}
        ProccessDict proccessDict = new ProccessDict(_logger, "processes.txt");
        proccessDict.Initialize();

        ProccessDict goodProccessDict = new ProccessDict(_logger, "goodprocesses.txt");
        goodProccessDict.Initialize();

	if (_args.Length > 0) {
		MessageJson messages = new MessageJson(_logger, _args[0]);
	}
        while (!stoppingToken.IsCancellationRequested)
        {
            proccessDict.Update();
            goodProccessDict.Update();
            CheckBadProcessesPlaytime(proccessDict.GetDict());
            CheckGoodProcessesPlaytime(goodProccessDict.GetDict());
            await Task.Delay(900000, stoppingToken);
        }
    }

    public void CheckBadProcessesPlaytime(Dictionary<string, TimeSpan> processesDict)
    {
        foreach (KeyValuePair<string, TimeSpan> entry in processesDict)
        {
            string processName = entry.Key;
            TimeSpan timeElapsed = entry.Value;
            if (timeElapsed.TotalMinutes > 30)
            {
                string subject = "wee woo wee woo";
                string body = "hey, get off " + processName;
                string to = "djx3rn@virginia.edu";
                SendEmail(subject, body, to);
            }
        }
    }

    public void CheckGoodProcessesPlaytime(Dictionary<string, TimeSpan> processesDict)
    {
        foreach (KeyValuePair<string, TimeSpan> entry in processesDict)
        {
            string processName = entry.Key;
            TimeSpan timeElapsed = entry.Value;
            if (timeElapsed.TotalMinutes > 30)
            {
                string subject = "remember to take a break!";
                string body = "take a break from " + processName;
                string to = "djx3rn@virginia.edu";
                SendEmail(subject, body, to);
            }
        }
    }

    private static readonly object emailLock = new object();
    
    public void SendEmail(string subject, string body, string to)
    {
        lock (emailLock)
    {   
        using (var smtpClient = new SMTPHelper().GetSmtpClient())
        using (var mailMessage = new MailMessage())
        {
            var fromMailAddress = new MailAddress("dependsonthesmtp@gmail.com", "tiger"); // some smtp overrides this but gmail is chill
            var toMailAddress = new MailAddress(to);
            mailMessage.From = fromMailAddress;
            mailMessage.To.Add(toMailAddress);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
        try
        {   
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email.");
        }
        }
    }
    }
}
