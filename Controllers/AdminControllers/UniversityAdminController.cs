using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniCompass.DTOs;
using UniCompass.DTOs.UniversityDtos;

namespace UniCompass.Controllers.AdminControllers
{
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class AdminUniversityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Supabase.Client _supabase;

        private readonly Cloudinary _cloudinary;

        public AdminUniversityController(
            IMapper mapper,
            [FromKeyedServices("admin")] Supabase.Client supabase,
            Cloudinary cloudinary
        )
        {
            _mapper = mapper;
            _supabase = supabase;
            _cloudinary = cloudinary;
        }

        [HttpPost("CreateUniversity")]
        public async Task<IActionResult> CreateUniversity(
            [FromForm] CreateUniversityDto createUniversityDto,
            IFormFile? universityPhoto
        )
        {
            if (createUniversityDto == null)
            {
                return BadRequest("Invalid university data.");
            }
            var university = _mapper.Map<Models.Universities>(createUniversityDto);

            if (universityPhoto == null)
                return Ok("University created successfully without a photo.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(universityPhoto.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Invalid file type. Only image files are allowed.");

            var fileName = $"{university.UniversityName}_{Guid.NewGuid()}{fileExtension}";

            var uploadResult = await _cloudinary.UploadAsync(
                new ImageUploadParams
                {
                    File = new FileDescription(fileName, universityPhoto.OpenReadStream()),
                    Folder = "universities",
                }
            );

            if (uploadResult.Error != null)
            {
                return BadRequest("Failed to upload university photo.");
            }

            university.PhotoUrl = uploadResult.Url.ToString();
            university.PhotoPublicId = uploadResult.PublicId;

            var response = await _supabase.From<Models.Universities>().Insert(university);

            if (response.Models.Count == 0)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Failed to create university in database."
                );
            }

            return Ok("University created successfully.");
        }

        [HttpPost("UpdateUniversityPhoto")]
        public async Task<IActionResult> UpdateUniversityPhoto(
            [FromForm] int universityId,
            IFormFile? universityPhoto
        )
        {
            if (universityPhoto == null)
            {
                return BadRequest("No photo provided.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(universityPhoto.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only image files are allowed.");
            }

            var fileName = $"{universityId}_{Guid.NewGuid()}{fileExtension}";

            var university = await _supabase
                .From<Models.Universities>()
                .Where(x => x.UniversityId == universityId)
                .Single();

            if (university == null)
                return NotFound("University not found.");

            if (university.PhotoUrl != null && university.PhotoUrl.Length > 0)
            {
                var deletionResponse = await _cloudinary.DestroyAsync(
                    new DeletionParams(university.PhotoPublicId)
                );

                if (deletionResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        "Failed to delete existing university photo."
                    );
                }
            }
            var uploadResult = await _cloudinary.UploadAsync(
                new ImageUploadParams
                {
                    File = new FileDescription(fileName, universityPhoto.OpenReadStream()),
                    Folder = "universities",
                }
            );

            if (uploadResult.Error != null)
            {
                return BadRequest("Failed to upload university photo.");
            }

            await _supabase
                .From<Models.Universities>()
                .Where(x => x.UniversityId == universityId)
                .Set(x => x.PhotoUrl, uploadResult.Url.ToString())
                .Set(x => x.PhotoPublicId, uploadResult.PublicId)
                .Update();

            return Ok("University photo updated successfully.");
        }

        [HttpGet("GetAllUniversities")]
        public async Task<IActionResult> GetAllUniversities()
        {
            var universities = await _supabase.From<Models.Universities>().Get();

            if (universities.Models.Count == 0)
            {
                return NotFound("No universities found.");
            }

            var universityDtos = _mapper.Map<List<UniversityDto>>(universities.Models);

            return Ok(universityDtos);
        }
    }
}
