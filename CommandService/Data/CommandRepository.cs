using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
  private readonly AppDbContext _dbContext;

  public CommandRepository(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task CreateCommandAsync(int platformId, Command command)
  {
    if(command == null)
    {
      throw new ArgumentNullException(nameof(command));
    }
    command.PlatformId = platformId;
    await _dbContext.AddAsync(command);
  }

  public async Task CreatePlatformAsync(Platform platform)
  {
    if(platform == null)
    {
      throw new ArgumentNullException(nameof(platform));
    }
    await _dbContext.Platforms.AddAsync(platform);
  }

  public async Task<bool> ExternalPlatformExistsAsync(int platformId)
  {
    return await _dbContext.Platforms.AnyAsync(p => p.ExternalId == platformId);
  }

  public async Task<Command?> GetCommandAsync(int platformId, int commandId)
  {
    return await _dbContext.Commands
      .Where(c => c.PlatformId == platformId)
      .Where(c => c.Id == commandId)
      .FirstOrDefaultAsync();
  }

  public async Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId)
  {
    return await _dbContext.Commands
      .Include(c => c.Platform)
      .Where(c => c.PlatformId == platformId)
      .ToListAsync();
  }

  public async Task<IEnumerable<Platform>> GetPlatformsAsync()
  {
    return await _dbContext.Platforms.ToListAsync();
  }

  public async Task<bool> PlatformExistsAsync(int platformId)
  {
    return await _dbContext.Platforms.AnyAsync(p => p.Id == platformId);
  }

  public async Task<bool> SaveChangesAsync()
  {
    return (await _dbContext.SaveChangesAsync()) >= 0;
  }
}
