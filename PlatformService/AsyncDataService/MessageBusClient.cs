using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataService;

public class MessageBusClient : IMessageBusClient, IDisposable
{
  private readonly ILogger<MessageBusClient> _logger;
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private bool _isDisposed;

  public MessageBusClient(IConfiguration configuration, ILogger<MessageBusClient> logger)
  {
    _logger = logger;
    var factory = new ConnectionFactory
    {
      HostName = configuration["RabbitMQHost"],
      Port = int.Parse(configuration["RabbitMQPort"])
    };
    try
    {
      _connection = factory.CreateConnection();
      _channel = _connection.CreateModel();
      _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
      _connection.ConnectionShutdown += OnConnectionShutdown;
      _logger.LogInformation("--> Connected to message bus");
    }
    catch(Exception ex)
    {
      _logger.LogError(ex, "Error connecting to message bus");
      throw;
    }
  }
  public void PublishNewPlatform(PlatformPublishDto platformPublishDto)
  {
    var message = JsonSerializer.Serialize(platformPublishDto);
    if (_connection.IsOpen)
    {
      _logger.LogInformation("--> Connection open, sending message...");
      SendMessage(message);
    }
    else
    {
      _logger.LogInformation("--> Connection closed");
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (!_isDisposed)
      {
        _logger.LogInformation("--> Disposing bus client");
        if (_channel.IsOpen)
        {
          _channel.Close();
          _connection.Close();
        }
        _isDisposed = true;
      }
    }
  }

  private void SendMessage(string message)
  {
    var body = Encoding.UTF8.GetBytes(message);
    _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body);
    _logger.LogInformation("--> Message sent: {message}", message);
  }

  private void OnConnectionShutdown(object? sender, ShutdownEventArgs? args)
  {
    _logger.LogInformation("--> Rabbit MQ connection shut down");
  }
}
