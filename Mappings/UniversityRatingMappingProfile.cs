using System;
using AutoMapper;
using UniCompass.DTOs.UniversityRatingDtos;
using UniCompass.Models;

namespace UniCompass.Mappings;

public class UniversityRatingMappingProfile : Profile
{
    public UniversityRatingMappingProfile()
    {
        CreateMap<PostUniversityRating, UniversityRatings>();
    }
}
