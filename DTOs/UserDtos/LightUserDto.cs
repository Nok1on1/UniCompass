namespace UniCompass.DTOs;

public class LightUserDto
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string UserType { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoPublicId { get; set; }
    public string? Bio { get; set; }
}
