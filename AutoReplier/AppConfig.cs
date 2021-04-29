namespace AutoReplier
{
    public class AppConfig
    {
        public string MailAddress { get; set; }

        public string Password { get; set; }

        public string ImapHost { get; set; }

        public int ImapPort { get; set; }

        public bool ImapSsl { get; set; }

        public int UpdatingIntervalDelayInSeconds { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public bool SmtpSsl { get; set; }

        public int SendingIntervalDelayInSeconds { get; set; }

        public string MailAddressFrom { get; set; }
    }
}
