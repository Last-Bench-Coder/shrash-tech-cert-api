using System;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using shrash_tech_certificate_gen_api.Entities;

public class EmailService
{
	private readonly string _gmailUser = "shrash.technology@gmail.com";
	private readonly string _gmailAppPassword = "ubsh rcdu dsuw zwnj";

	public async Task SendCertificateEmailAsync(string toEmail, string downloadURL, Certificates certificate)
	{
		var message = new MimeMessage();

		message.From.Add(new MailboxAddress(certificate.InstituteName ?? "Shrash Tech Academy", _gmailUser));
		message.To.Add(MailboxAddress.Parse(toEmail));
		message.Subject = "🎓 Your Certificate is Ready!";

		var htmlBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
          <div style='background-color: #f9f9f9; padding: 20px; text-align: center;'>
            <img src='{certificate.InstituteLogo}' alt='Institute Logo' style='max-width: 120px; height: auto;' />
            <h2 style='margin-top: 10px; color: #333;'>{certificate.InstituteName}</h2>
          </div>
          <div style='padding: 30px; color: #333;'>
            <p>Dear {certificate.StudentName ?? "Learner"},</p>
            <p style='font-size: 16px;'>Congratulations on successfully completing the <strong>{certificate.CourseName}</strong> course!</p>
            <p>Your certificate is now ready for download (Certificate Id: {certificate.CertificateId}).</p>
            <div style='text-align: center; margin: 30px 0;'>
              <a href='{downloadURL}' target='_blank' style='background-color: #007BFF; color: #fff; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Download Certificate</a>
            </div>
            <p>If you have any questions, feel free to contact us.</p>
            <p><b>Best Regards</b>,<br/>{certificate.SignatureName ?? "Shrash Tech Team"}</p>
          </div>
          <div style='background-color: #f1f1f1; padding: 20px; text-align: center; font-size: 13px; color: #666;'>
            <div>{certificate.InstituteAddress}</div>
            <div style='margin-top: 5px;'>Phone: {certificate.InstitutePhone} | Email: <a href='mailto:{certificate.InstituteEmail}' style='color: #555;'>{certificate.InstituteEmail}</a></div>
          </div>
        </div>";

		var bodyBuilder = new BodyBuilder
		{
			HtmlBody = htmlBody
		};

		message.Body = bodyBuilder.ToMessageBody();

		using var client = new MailKit.Net.Smtp.SmtpClient();
		await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
		await client.AuthenticateAsync(_gmailUser, _gmailAppPassword);
		await client.SendAsync(message);
		await client.DisconnectAsync(true);
	}
}
