using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using UniCompass.DTOs;

namespace UniCompass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Supabase.Client _userClient;

        private readonly Cloudinary _cloudinary;

        public UserController(
            IMapper mapper,
            [FromKeyedServices("user")] Supabase.Client supabase,
            Cloudinary cloudinary
        )
        {
            _mapper = mapper;
            _userClient = supabase;
            _cloudinary = cloudinary;
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userClient.From<Models.Users>().Get();
            var userDtos = _mapper.Map<List<UserDto>>(response.Models);

            return Ok(userDtos);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var response = await _userClient.From<Models.Users>().Where(x => x.UserId == id).Get();

            var user = response.Models.FirstOrDefault();

            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var response = await _userClient
                .From<Models.Users>()
                .Where(x => x.Email == email)
                .Get();

            var userDto = _mapper.Map<UserDto>(response.Models.FirstOrDefault());

            return Ok(userDto);
        }

        [HttpPut("UpdateUserPhoto")]
        public async Task<IActionResult> UpdateUserPhoto(
            Guid userId,
            [FromForm] IFormFile? profileImage = null
        )
        {
            var uploadResult = new ImageUploadResult();

            var userDb = await _userClient
                .From<Models.Users>()
                .Where(x => x.UserId == userId)
                .Get();

            var user = _mapper.Map<Models.Users>(userDb.Models.FirstOrDefault());

            if (profileImage != null && profileImage.Length > 0)
            {
                if (user.ProfilePictureUrl != null)
                {
                    // Delete existing profile picture if it exists
                    var deleteParams = new DeletionParams(user.ProfilePicturePublicId);

                    var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                    if (deleteResult.StatusCode != System.Net.HttpStatusCode.OK)
                        return StatusCode(
                            500,
                            $"Failed to delete existing profile picture: {deleteResult.Error.Message}"
                        );
                }

                // Validate file type (optional)
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only image files are allowed.");
                }

                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";

                // Upload to Cloudinary
                using var stream = profileImage.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill"),
                    Folder = "profile-images",
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var url = uploadResult.Url.ToString();
                var publicId = uploadResult.PublicId;

                await _userClient
                    .From<Models.Users>()
                    .Where(x => x.UserId == userId)
                    .Set(x => x.ProfilePictureUrl, url)
                    .Set(x => x.ProfilePicturePublicId, publicId)
                    .Update();

                return Ok();
            }
            else
            {
                await _userClient
                    .From<Models.Users>()
                    .Where(x => x.UserId == userId)
                    .Set(x => x.ProfilePictureUrl, "")
                    .Set(x => x.ProfilePicturePublicId, "")
                    .Update();

                return StatusCode(500, "Failed to upload profile picture.");
            }
        }
    }
}
