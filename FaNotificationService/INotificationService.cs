using System.ServiceModel;

namespace FaNotificationService
{
    [ServiceContract]
    public interface INotificationService
    {
        [OperationContract]
        void Send(ESenders senderType, CMessage message);
    }
    
}
