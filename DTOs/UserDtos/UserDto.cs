namespace UniCompass.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string UserType { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PhotoPublicId { get; set; }
        public string? Bio { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
