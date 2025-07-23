namespace UniCompass.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string UserType { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public bool? IsVerified { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePicturePublicId { get; set; }
        public string? Bio { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
