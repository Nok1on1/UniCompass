using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UniCompass.Controllers.AdminControllers
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class UserAdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Supabase.Client _supabase;
        private readonly Cloudinary _cloudinary;

        public UserAdminController(
            IMapper mapper,
            [FromKeyedServices("admin")] Supabase.Client supabase,
            Cloudinary cloudinary
        )
        {
            _mapper = mapper;
            _supabase = supabase;
            _cloudinary = cloudinary;
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await _supabase.From<Models.Users>().Where(x => x.UserId == userId).Single();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var photoDeleteResponse = await _cloudinary.DeleteResourcesAsync(user.PhotoPublicId);

            if (photoDeleteResponse.Error != null)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Error deleting user photo."
                );
            }

            await _supabase.From<Models.Users>().Where(x => x.UserId == userId).Delete();

            return Ok("User deleted successfully.");
        }
    }
}
