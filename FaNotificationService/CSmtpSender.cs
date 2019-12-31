using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using NLog;

namespace FaNotificationService
{
    public class CSmtpSender : ISender
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly CSmtpSettings _settings;

        public CSmtpSender()
        {
            try
            {
                _settings = new CSmtpSettings
                {
                    Address =  ConfigurationManager.AppSettings["smtpAddress"],
                    Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]),
                    User = ConfigurationManager.AppSettings["smtpCredUser"],
                    Password = ConfigurationManager.AppSettings["smtpCredPswd"],
                    EnableSsl =  Boolean.Parse(ConfigurationManager.AppSettings["smtpEnableSsl"])
                };
            }
            catch (Exception ex)
            {
                s_logger.Error($"An error occured while reading smtp settings: {ex}");
                throw;
            }
        }

        public void Send(CMessage message)
        {
            var from = new MailAddress(_settings.User);
            var to = new MailAddress(message.Receiver);
            var mailMessage = new MailMessage(from, to) { Subject = message.Subject, Body = message.Body };

            try
            {
                using (var smtpClient = new SmtpClient(_settings.Address, _settings.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(_settings.User, _settings.Password);
                    smtpClient.EnableSsl = _settings.EnableSsl;
                    // smtpClient.Send(_message);
                    s_logger.Info($"The message {mailMessage.Subject} was sent: {mailMessage.Sender}");
                };
            }
            catch (Exception ex)
            {
                s_logger.Error($"An error occured while sending message: {ex}");
                throw;
            }
        }
    }
}
