
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Taxi_App;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }
    public async Task SendEmail(string email, string verification)
    {
        var mail = new MimeMessage();
        string text = $"Your verification request for Taxi App is - {verification}";

        mail.From.Add(MailboxAddress.Parse(_config["MailSettings:From"]));
        mail.To.Add(MailboxAddress.Parse(email));
        mail.Subject = "Verification";
        mail.Body = new TextPart(TextFormat.Html) { Text = text };

        using var smtp = new SmtpClient();
        smtp.CheckCertificateRevocation = false;
        await smtp.ConnectAsync(_config["MailSettings:Host"], 587, SecureSocketOptions.Auto);
        await smtp.AuthenticateAsync(_config["MailSettings:From"], _config["MailSettings:Password"]);
        await smtp.SendAsync(mail);
        await smtp.DisconnectAsync(true);
    }
}
