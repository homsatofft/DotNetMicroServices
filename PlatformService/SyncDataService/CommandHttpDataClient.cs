using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataService
{
  public class CommandHttpDataClient : ICommandHttpDataClient
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<CommandHttpDataClient> _logger;
    private readonly string _commandServiceUrl;

    public CommandHttpDataClient(HttpClient client, ILogger<CommandHttpDataClient> logger, IConfiguration configuration)
    {
      _httpClient = client;
      _logger = logger;
      _commandServiceUrl = configuration["CommandServiceUrl"];
    }
    public async Task SendPlatformToCommandAsync(PlatformReadDto platform)
    {
      var httpContent = new StringContent(
        JsonSerializer.Serialize(platform),
        Encoding.UTF8,
        "application/json");
      var response = await _httpClient.PostAsync($"{_commandServiceUrl}/api/c/platforms", httpContent);
      if (response.IsSuccessStatusCode)
      {
        _logger.LogInformation("--> POST to Command service OK");
      }
      else
      {
        _logger.LogError("--> POST to Command service returned Error {statusCode}", response.StatusCode);
      }
    }
  }
}
