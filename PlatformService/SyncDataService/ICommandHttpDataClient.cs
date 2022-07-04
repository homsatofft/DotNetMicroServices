using PlatformService.Dtos;

namespace PlatformService.SyncDataService
{
  public interface ICommandHttpDataClient
  {
    Task SendPlatformToCommandAsync(PlatformReadDto platform);
  }
}
