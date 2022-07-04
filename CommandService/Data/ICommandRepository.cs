using CommandService.Models;

namespace CommandService.Data;

public interface ICommandRepository
{
  Task<bool> SaveChangesAsync();
  // Platfroms
  Task<IEnumerable<Platform>> GetPlatformsAsync();
  Task CreatePlatformAsync(Platform platform);
  Task<bool> PlatformExistsAsync(int platformId);
  Task<bool> ExternalPlatformExistsAsync(int platformId);

  //Commands
  Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId);
  Task<Command?> GetCommandAsync(int platformId, int commandId);
  Task CreateCommandAsync(int platformId, Command command);
}
