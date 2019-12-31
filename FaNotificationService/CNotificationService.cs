using NLog;
using System.ServiceModel;

namespace FaNotificationService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CNotificationService : INotificationService
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public void Send(ESenders senderType, CMessage message)
        {
            if (senderType == ESenders.Smtp)
            {
                CSmtpSender sender = new CSmtpSender();
                sender.Send(message);
            }
            else
            {
                s_logger.Warn($"Unsupported senderType: {senderType.ToString()}");
            }
        }
    }
}
