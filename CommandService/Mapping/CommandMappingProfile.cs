using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Mapping;

public class CommandMappingProfile : Profile
{
  public CommandMappingProfile()
  {
    CreateMap<Platform, PlatformReadDto>();
    CreateMap<Command, CommandReadDto>();
    CreateMap<CommandCreateDto, Command>();
    CreateMap<PlatformPublishDto, Platform>()
      .ForMember(dst => dst.ExternalId, opt => opt.MapFrom(src => src.Id))
      .ForMember(dst => dst.Id, opt => opt.Ignore());
    CreateMap<GrpcPlatformModel, Platform>()
      .ForMember(dst => dst.ExternalId, opt => opt.MapFrom(src => src.PlatformId));
  }
}
