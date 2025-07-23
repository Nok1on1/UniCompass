using AutoMapper;
using UniCompass.Models;

namespace UniCompass.DTOs;

public class userMappingProfile : Profile
{
    public userMappingProfile()
    {
        CreateMap<Users, UserDto>();
        CreateMap<Users, LightUserDto>();
    }
}
