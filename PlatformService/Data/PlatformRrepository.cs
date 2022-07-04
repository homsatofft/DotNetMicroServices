using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRrepository : IPlatformRepository
{
  private readonly AppDbContext _dbContext;

  public PlatformRrepository(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task CreatePlatformAsync(Platform platform)
  {
    if(platform == null)
    {
      throw new ArgumentNullException(nameof(platform));
    }
    await _dbContext.Platforms.AddAsync(platform);
  }

  public async Task<Platform?> GetPlatformByIdAsync(int id)
  {
    return await _dbContext.Platforms.FirstOrDefaultAsync(p => p.Id == id);
  }

  public async Task<IEnumerable<Platform>> GetPlatformsAsync()
  {
    return await _dbContext.Platforms.ToListAsync();
  }

  public async Task<bool> SaveChangesAsync()
  {
    return (await _dbContext.SaveChangesAsync()) >= 0;
  }
}
