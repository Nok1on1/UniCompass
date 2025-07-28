using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Supabase.Gotrue;
using UniCompass.DTOs;

namespace UniCompass.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Supabase.Client _supabase;
        private readonly IMapper _mapper;

        public AuthController([FromKeyedServices("user")] Supabase.Client supabase, IMapper imapper)
        {
            _supabase = supabase;
            _mapper = imapper;
        }

        /// <summary>
        /// Registers a new user using the provided registration details.
        /// This endpoint is intended for backend use only; frontend applications should implement Supabase authentication directly.
        /// </summary>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var signUpResponse = await _supabase.Auth.SignUp(
                registerDto.Email,
                registerDto.Password
            );

            if (signUpResponse.User.Id == null)
            {
                return BadRequest("Registration failed. Please try again.");
            }

            return Ok("User registered successfully.");
        }

        /// <summary>
        /// This controller is intended for backend use only; frontend applications should interact with Supabase authentication directly.
        /// </summary>
        /// <returns>Token Cookies</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RegisterDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            try
            {
                var session = await _supabase.Auth.SignIn(loginDto.Email, loginDto.Password);

                if (session?.User == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                var accessToken = session.AccessToken;
                var refreshToken = session.RefreshToken;

                Response.Cookies.Append(
                    "x-access-token",
                    accessToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddSeconds(5),
                    }
                );

                Response.Cookies.Append(
                    "x-refresh-token",
                    refreshToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                    }
                );

                return Ok("User logged in successfully.");
            }
            catch (Exception ex)
            {
                return Unauthorized($"Login failed: {ex.Message}");
            }
        }

        /// <summary>
        /// This controller is intended for backend use only; frontend applications should interact with Supabase authentication directly.
        /// </summary>
        /// <returns>Token Cookies</returns>
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _supabase.Auth.SignOut();

            Response.Cookies.Delete("access-token");
            Response.Cookies.Delete("refresh-token");

            return Ok("User logged out successfully.");
        }

        [HttpPost("InfoDump")]
        public async Task<IActionResult> InfoDump([FromBody] CreateUserDto createUserDto)
        {
            var currentUser = _supabase.Auth.CurrentUser;

            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            if (createUserDto.UserType == "ADMIN")
            {
                // Flag the user for admin review or log the request
                // For example, you could log this event or set a property in the database
                // Here, just return a message for demonstration
                return Forbid(
                    "Admin registration requires approval. Your request has been flagged."
                );
            }

            if (!Guid.TryParse(currentUser.Id, out var userGuid))
            {
                return BadRequest("Invalid user ID format.");
            }

            // Check if user already exists
            var existingUserResponse = await _supabase
                .From<Models.Users>()
                .Where(x => x.UserId == userGuid)
                .Get();

            if (existingUserResponse.Models?.Any() == true)
            {
                return Conflict("User profile already exists.");
            }

            var user = _mapper.Map<Models.Users>(createUserDto);

            try
            {
                // Insert to database
                var response = await _supabase.From<Models.Users>().Insert(user);

                if (response.Models?.Any() != true)
                {
                    return StatusCode(500, "Failed to create user profile.");
                }

                var insertedUser = response.Models.First();

                // Map entity back to DTO for response
                var userDto = _mapper.Map<UserDto>(insertedUser);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(user, Formatting.Indented));
                return StatusCode(500, $"Error creating user profile: {ex.Message}");
            }
        }
    }
}
