using AutoMapper;
using UniCompass.Models;

namespace UniCompass.DTOs;

public class userMappingProfile : Profile
{
    public userMappingProfile()
    {
        CreateMap<Users, UserDto>();
        CreateMap<Users, LightUserDto>();
        CreateMap<CreateUserDto, Users>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}
