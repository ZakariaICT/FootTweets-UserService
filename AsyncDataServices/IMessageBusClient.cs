using UserService.DTO;

namespace UserService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewUser(UserPublishedDto userPublishedDto);
    }
}