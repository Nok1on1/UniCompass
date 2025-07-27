using System;

namespace UniCompass.DTOs;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string UserType { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Bio { get; set; }
}
