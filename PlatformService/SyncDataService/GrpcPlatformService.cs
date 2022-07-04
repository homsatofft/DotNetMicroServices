using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataService;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
  private readonly IPlatformRepository _platformRepository;
  private readonly IMapper _mapper;
  private readonly ILogger<GrpcPlatformService> _logger;

  public GrpcPlatformService(IPlatformRepository platformRepository, IMapper mapper, ILogger<GrpcPlatformService> logger)
  {
    _platformRepository = platformRepository;
    _mapper = mapper;
    _logger = logger;
  }

  public override async Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
  {
    _logger.LogInformation("--> Requesting platforms");
    var response = new PlatformResponse();
    var platforms = (await _platformRepository.GetPlatformsAsync());
    foreach(var platform in platforms)
    {
      response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
    }
    return response;
  }
}
