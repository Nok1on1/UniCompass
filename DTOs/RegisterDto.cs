using System;

namespace UniCompass.DTOs;

public class RegisterDto
{
    public string? Email { get; set; }
    public required string Password { get; set; }
}
