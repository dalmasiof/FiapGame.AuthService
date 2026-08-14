using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Text.Json;

namespace Messaging;

public class RabbitMqPublisher : IMessagePublisher, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ServiceBusClient? _serviceBusClient;
    private readonly ServiceBusSender? _notificationSender;
    private IConnection? _connection;
    private IChannel? _channel;

    private const string ExchangeName = "notificacao.exchange";

    public RabbitMqPublisher(IConnectionFactory connectionFactory, IConfiguration configuration, IHostEnvironment environment)
    {
        _connectionFactory = connectionFactory;
        var fullyQualifiedNamespace = configuration["ServiceBus:FullyQualifiedNamespace"];
        if (!string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
        {
            _serviceBusClient = new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential());
            _notificationSender = _serviceBusClient.CreateSender("notification-authentication");
        }
        else if (environment.IsProduction())
        {
            throw new InvalidOperationException("ServiceBus:FullyQualifiedNamespace is required in Production.");
        }
    }

    private async Task ConnectAsync()
    {
        if (_channel is not null) return;
        _connection = await _connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
    }

    public async Task PublishAsync<T>(T message, string routingKey, string correlationId) where T : class
    {
        await ConnectAsync();
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            CorrelationId = correlationId,
            ContentEncoding = "utf-8"
        };

        await _channel!.BasicPublishAsync(ExchangeName, routingKey, mandatory: false, properties, body);
        if (_notificationSender is not null)
        {
            await _notificationSender.SendMessageAsync(new ServiceBusMessage(body)
            {
                ContentType = "application/json",
                CorrelationId = correlationId,
                MessageId = correlationId
            });
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_notificationSender is not null) await _notificationSender.DisposeAsync();
        if (_serviceBusClient is not null) await _serviceBusClient.DisposeAsync();
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
