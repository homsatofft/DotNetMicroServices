using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.AsyncDataService;

public class MessageBusSubscriber : BackgroundService, IDisposable
{
  private readonly IEventProcessor _eventProcessor;
  private readonly ILogger<MessageBusSubscriber> _logger;
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private readonly string _queueName;
  private bool _isDisposed;

  public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor, ILogger<MessageBusSubscriber> logger)
  {
    _eventProcessor = eventProcessor;
    _logger = logger;
    var factory = new ConnectionFactory
    {
      HostName = configuration["RabbitMQHost"],
      Port = int.Parse(configuration["RabbitMQPort"])
    };
    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();
    _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
    _queueName = _channel.QueueDeclare().QueueName;
    _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
    _logger.LogInformation("--> Listening the Message Bus");
    _connection.ConnectionShutdown += OnConnectionShutdown;
  }
  protected override Task ExecuteAsync(CancellationToken stoppingToken)
  {
    stoppingToken.ThrowIfCancellationRequested();
    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += async (ModuleHandle, ea) =>
    {
      _logger.LogInformation("--> Event received");
      var body = ea.Body;
      var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
      await _eventProcessor.ProcessEventAsync(notificationMessage);
    };
    _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    return Task.CompletedTask;
  }

  public override void Dispose()
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

  private void OnConnectionShutdown(object? sender, ShutdownEventArgs? args)
  {
    _logger.LogInformation("--> Rabbit MQ connection shut down");
  }
}
