﻿namespace FaNotificationService
{
    public class CSmtpSettings
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
