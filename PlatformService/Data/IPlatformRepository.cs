using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformRepository
{
  Task<bool> SaveChangesAsync();
  Task<IEnumerable<Platform>> GetPlatformsAsync();
  Task<Platform?> GetPlatformByIdAsync(int id);
  Task CreatePlatformAsync(Platform platform);
}
