namespace ERP_Models.Common.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message);
    }
}
