using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/platforms")]
[ApiController]
public class PlatformsController : ControllerBase
{
  private readonly ILogger<PlatformsController> _logger;
  private readonly ICommandRepository _commandRepository;
  private readonly IMapper _mapper;

  public PlatformsController(ILogger<PlatformsController> logger, ICommandRepository commandRepository, IMapper mapper)
  {
    _logger = logger;
    _commandRepository = commandRepository;
    _mapper = mapper;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
  {
    _logger.LogInformation("--> Getting platforms from CommandService");
    var platformItems = await _commandRepository.GetPlatformsAsync();
    return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
  }

  [HttpPost]
  public IActionResult Test()
  {
    _logger.LogInformation("--> Calling test");
    return Ok("commands service platforms controller inbound ok");
  }
}
