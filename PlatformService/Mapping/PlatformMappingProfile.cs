using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Mapping;

public class PlatformMappingProfile : Profile
{
  public PlatformMappingProfile()
  {
    CreateMap<PlatformCreateDto, Platform>();
    CreateMap<Platform, PlatformReadDto>();
    CreateMap<PlatformReadDto, PlatformPublishDto>();
    CreateMap<Platform, GrpcPlatformModel>()
      .ForMember(dst => dst.PlatformId, opt => opt.MapFrom(src => src.Id));
  }
}
