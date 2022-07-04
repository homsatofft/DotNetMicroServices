using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/platforms/{platformId}/commands")]
[ApiController]
public class CommandsController : ControllerBase
{
  private readonly ICommandRepository _commandRepository;
  private readonly IMapper _mapper;
  private readonly ILogger<CommandsController> _logger;
  public CommandsController(ICommandRepository command, IMapper mapper, ILogger<CommandsController> logger)
  {
    _commandRepository = command;
    _mapper = mapper;
    _logger = logger;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
  {
    _logger.LogInformation("--> Getting all commands for platform {platformId}", platformId);
    if(!await _commandRepository.PlatformExistsAsync(platformId))
    {
      return NotFound();
    }
    var commands = await _commandRepository.GetCommandsForPlatform(platformId);
    return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
  }

  [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
  public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
  {
    _logger.LogInformation("--> Getting command for platform {platformId}/{commandId}", platformId, commandId);
    if (!await _commandRepository.PlatformExistsAsync(platformId))
    {
      return NotFound();
    }
    var command = await _commandRepository.GetCommandAsync(platformId, commandId);
    if(command == null)
    {
      return NotFound();
    }
    return Ok(_mapper.Map<CommandReadDto>(command));
  }

  [HttpPost]
  public async Task<ActionResult<CommandReadDto>> CreateCommand(int platformId, CommandCreateDto createCommandDto)
  {
    _logger.LogInformation("--> Creating command for platform {platformId}", platformId);
    if (!await _commandRepository.PlatformExistsAsync(platformId))
    {
      return NotFound();
    }
    var command = _mapper.Map<Command>(createCommandDto);
    await _commandRepository.CreateCommandAsync(platformId, command);
    await _commandRepository.SaveChangesAsync();
    var commandReadId = _mapper.Map<CommandReadDto>(command);
    return CreatedAtRoute(nameof(GetCommandForPlatform), 
      new { platformId, commandId = commandReadId.Id }, 
      commandReadId);
  }
}
