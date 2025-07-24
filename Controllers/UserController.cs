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

        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = _userClient.Auth.CurrentUser;

            if (response == null)
                return Unauthorized("User not authenticated.");

            if (!Guid.TryParse(response.Id, out var userGuid))
                return Unauthorized("Invalid user ID.");

            var userDb = await _userClient
                .From<Models.Users>()
                .Where(x => x.UserId == userGuid)
                .Single();

            if (userDb == null)
                return NotFound("User not found.");

            var userDto = _mapper.Map<UserDto>(userDb);

            return Ok(userDto);
        }

        [HttpPut("UpdateUserPhoto")]
        public async Task<IActionResult> UpdateUserPhoto(IFormFile? profileImage)
        {
            var userId = _userClient.Auth.CurrentUser?.Id;

            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized("Invalid user ID.");

            var userDb = await _userClient
                .From<Models.Users>()
                .Where(x => x.UserId == userGuid)
                .Get();

            var user = _mapper.Map<UserDto>(userDb.Models.FirstOrDefault());

            var uploadResult = new ImageUploadResult();

            if (profileImage != null && profileImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file type. Only image files are allowed.");

                if (user.PhotoUrl != null)
                {
                    // Delete existing photo if it exists
                    var deleteParams = new DeletionParams(user.PhotoPublicId);

                    var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

                    if (deleteResult.StatusCode != System.Net.HttpStatusCode.OK)
                        return StatusCode(
                            500,
                            $"Failed to delete existing photo: {deleteResult.Error.Message}"
                        );
                }

                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";

                // Upload to Cloudinary
                using var stream = profileImage.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Transformation = new Transformation()
                        .Height(400)
                        .Width(400)
                        .Crop("fill")
                        .Quality("auto"),
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
                    .Where(x => x.UserId == userGuid)
                    .Set(x => x.PhotoUrl, url)
                    .Set(x => x.PhotoPublicId, publicId)
                    .Update();

                return Ok(
                    new
                    {
                        Message = "Profile photo updated successfully.",
                        PhotoUrl = url,
                        PhotoPublicId = publicId,
                    }
                );
            }
            else
            {
                await _userClient
                    .From<Models.Users>()
                    .Where(x => x.UserId == userGuid)
                    .Set(x => x.PhotoUrl, "")
                    .Set(x => x.PhotoPublicId, "")
                    .Update();

                return StatusCode(500, "Failed to upload photo.");
            }
        }
    }
}
