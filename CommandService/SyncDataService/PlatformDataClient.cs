using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataService;

public class PlatformDataClient : IPlatformDataClient
{
  private readonly IMapper _mapper;
  private readonly ILogger<PlatformDataClient> _logger;
  private readonly string _grpcPlatform;
  public PlatformDataClient(IConfiguration configuration, IMapper mapper, ILogger<PlatformDataClient> logger)
  {
    _grpcPlatform = configuration["GrpcPlatform"];
    _mapper = mapper;
    _logger = logger;
  }
  public async Task<IEnumerable<Platform>> GetAllPlatformsAsync()
  {
    _logger.LogInformation("--> Calling Grpc platform {platform}", _grpcPlatform);
    var channel = GrpcChannel.ForAddress(_grpcPlatform);
    var client = new GrpcPlatform.GrpcPlatformClient(channel);
    var request = new GetAllRequest();

    try
    {
      var reply = await client.GetAllPlatformsAsync(request);
      return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
    }
    catch(Exception ex)
    {
      _logger.LogError(ex, "Could not call Grpc Server {server}", _grpcPlatform);
      return Enumerable.Empty<Platform>();
    }
  }
}
