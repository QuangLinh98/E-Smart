namespace E_Smart.Mail
{
	public class EmailSetting
	{
		public string FromEmail { get; set; }
		public string FromPassword { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public bool EnableSsl { get; set; }
	}
}
