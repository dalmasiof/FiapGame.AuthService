namespace Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string routingKey, string correlationId) where T : class;
}