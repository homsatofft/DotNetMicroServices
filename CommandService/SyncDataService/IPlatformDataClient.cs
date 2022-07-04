using CommandService.Models;

namespace CommandService.SyncDataService;

public interface IPlatformDataClient
{
  Task<IEnumerable<Platform>> GetAllPlatformsAsync();
}
