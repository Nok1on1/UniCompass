using System;

namespace UniCompass.DTOs.UniversityDtos;

public class CreateUniversityDto
{
    public required string UniversityName { get; set; }

    public required string Location { get; set; }

    public DateOnly? EstablishedDate { get; set; }

    public String? Bio { get; set; }
}
