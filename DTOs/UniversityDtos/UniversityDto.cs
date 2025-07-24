using System;

namespace UniCompass.DTOs;

public class UniversityDto
{
    public int UniversityId { get; set; }

    public string? UniversityName { get; set; }

    public string? Location { get; set; }

    public DateOnly? EstablishedDate { get; set; }

    public string? PhotoUrl { get; set; }

    public string? PhotoPublicId { get; set; }

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }
}
