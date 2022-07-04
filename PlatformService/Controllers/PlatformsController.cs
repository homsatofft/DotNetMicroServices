using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService;

namespace PlatformService.Controllers
{
  [Route("api/platforms")]
  [ApiController]
  public class PlatformsController : ControllerBase
  {
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ICommandHttpDataClient _commandHttpDataClient;
    private readonly ILogger<PlatformsController> _logger;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
      IPlatformRepository platformRepository,
      IMapper mapper,
      ICommandHttpDataClient commandHttpDataClient,
      ILogger<PlatformsController> logger,
      IMessageBusClient messageBusClient)
    {
      _platformRepository = platformRepository;
      _mapper = mapper;
      _commandHttpDataClient = commandHttpDataClient;
      _logger = logger;
      _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
    {
      var platforms = await _platformRepository.GetPlatformsAsync();
      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatform")]
    public async Task<ActionResult<PlatformReadDto>> GetPlatform(int id)
    {
      var platform = await _platformRepository.GetPlatformByIdAsync(id);
      if(platform == null)
      {
        return NotFound();
      }
      return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto platformCreateDto)
    {
      var platform = _mapper.Map<Platform>(platformCreateDto);
      await _platformRepository.CreatePlatformAsync(platform);
      await _platformRepository.SaveChangesAsync();
      var platformRead = _mapper.Map<PlatformReadDto>(platform);
      try
      {
        await _commandHttpDataClient.SendPlatformToCommandAsync(platformRead);
      }
      catch(Exception ex)
      {
        _logger.LogError("Could not send synchronously: {errorMessage}", ex.Message);
      }
      try
      {
        var dto = _mapper.Map<PlatformPublishDto>(platformRead);
        dto.Event = "Platform_Published";
        _messageBusClient.PublishNewPlatform(dto);
      }
      catch(Exception ex)
      {
        _logger.LogError("Could not send asynchronously: {errorMessage}", ex.Message);
      }
      return CreatedAtRoute(nameof(GetPlatform), new { Id = platformRead.Id }, platformRead);
    }
  }
}
