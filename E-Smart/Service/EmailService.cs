using System.Net.Mail;
using System.Net;
using E_Smart.Mail;
using Microsoft.Extensions.Options;

namespace E_Smart.Service
{
	public class EmailService
	{
		private readonly EmailSetting _emailSetting;

		public EmailService(IOptions<EmailSetting> emailSetting)
		{
			_emailSetting = emailSetting.Value;   // Vì trong program mình không có cấu hình option nên cần sử dung IOption và .Value để lấy giá trị 
		}

		public async Task SendEmail(string toEmail, string subject, string HtmlContent)
		{
			var fromAddress = new MailAddress(_emailSetting.FromEmail,"E-Smart");
			var toAddress = new MailAddress(toEmail);

			var smtp = new SmtpClient
			{
				Host = _emailSetting.Host,
				Port = _emailSetting.Port,
				EnableSsl = _emailSetting.EnableSsl,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new NetworkCredential(_emailSetting.FromEmail, _emailSetting.FromPassword)
			};

			using var message = new MailMessage(fromAddress, toAddress)
			{
				Subject = subject,
				Body = HtmlContent,
				IsBodyHtml = true // Đánh dấu nội dung email là HTML
			};
			await smtp.SendMailAsync(message);
		}
	}
}
