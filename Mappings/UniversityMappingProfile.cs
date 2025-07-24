using System;
using AutoMapper;
using UniCompass.DTOs;
using UniCompass.DTOs.UniversityDtos;
using UniCompass.Models;

namespace UniCompass.Mappings;

public class UniversityMappingProfile : Profile
{
    public UniversityMappingProfile()
    {
        CreateMap<CreateUniversityDto, Universities>()
            .ForMember(
                dest => dest.EstablishedDate,
                opt =>
                    opt.MapFrom(
                        (src) =>
                            src.EstablishedDate.HasValue
                                ? src.EstablishedDate.Value.ToDateTime(new TimeOnly(0, 0))
                                : (DateTime?)null
                    )
            );

        CreateMap<Universities, UniversityDto>()
            .ForMember(
                dest => dest.EstablishedDate,
                opt =>
                    opt.MapFrom(
                        (src) =>
                            src.EstablishedDate.HasValue
                                ? DateOnly.FromDateTime(src.EstablishedDate.Value)
                                : (DateOnly?)null
                    )
            );
    }
}
