using Infrastructure.Messenger;

namespace Infrastructure
{
    public interface IController
    {
        CMediator Mediator { get; }
    }
}