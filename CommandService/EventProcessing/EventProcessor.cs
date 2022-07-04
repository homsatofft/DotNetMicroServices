using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using System.Text.Json;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
  private readonly IMapper _mapper;
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILogger<EventProcessor> _logger;
  public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<EventProcessor> logger)
  {
    _scopeFactory = scopeFactory;
    _mapper = mapper;
    _logger = logger;
  }
  public async Task ProcessEventAsync(string message)
  {
    var eventType = DetermineEvent(message);
    switch (eventType)
    {
      case EventType.PlatformPublished:
        await AddPlatformAsync(message);
        break;

      default:
        break;
    }
  }

  private EventType DetermineEvent(string notificationMessage)
  {
    _logger.LogInformation("--> Determining event {event}", notificationMessage);
    var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
    if (eventType == null)
    {
      return EventType.Undefined;
    }
    switch (eventType.Event)
    {
      case "Platform_Published":
        {
          _logger.LogInformation("--> Platform publish event");
          return EventType.PlatformPublished;
        }
      default:
        return EventType.Undefined;
    }
  }

  private async Task AddPlatformAsync(string platformPublishedMessage)
  {
    _logger.LogInformation("--> Adding platform {platform}", platformPublishedMessage);
    using var scope = _scopeFactory.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
    var platformDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);
    try
    {
      var platform = _mapper.Map<Platform>(platformDto);
      if (!await repo.ExternalPlatformExistsAsync(platform.ExternalId))
      {
        await repo.CreatePlatformAsync(platform);
        await repo.SaveChangesAsync();
      }
      else
      {
        _logger.LogInformation("--> Platform already exists");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "--> Could not add platform");
    }
  }
}
