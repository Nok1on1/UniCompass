using System;

namespace UniCompass.DTOs.UniversityRatingDtos;

public class PostUniversityRating
{
    public int UniversityId { get; set; }
    public decimal RatingValue { get; set; }

    public string? Comment { get; set; }
}
