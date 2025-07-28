using System.Text.Json;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniCompass.DTOs;
using UniCompass.DTOs.UniversityDtos;
using UniCompass.Models;

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

        /// <summary>
        /// Imports universities and their courses from a JSON file
        /// </summary>
        /// <remarks>
        /// The JSON file should have the following structure:
        /// {
        ///   "universities": [
        ///     {
        ///       "uniId": "string",
        ///       "name": "string",
        ///       "subjects": [
        ///         {
        ///           "subjectId": "string",
        ///           "name": "string",
        ///           "price": "string"
        ///         }
        ///       ]
        ///    }
        ///   ]
        /// }
        /// </remarks>
        /// <returns>Import result with processing statistics</returns>
        [HttpPost("InsertUniversities")]
        public async Task<IActionResult> InsertUniversities(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
                return BadRequest("No file uploaded.");

            if (
                !Path.GetExtension(jsonFile.FileName)
                    .Equals(".json", StringComparison.OrdinalIgnoreCase)
            )
            {
                return BadRequest("Please upload a valid JSON file.");
            }

            List<Models.Universities>? universities;
            string jsonContent;
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                jsonContent = await reader.ReadToEndAsync();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
            };

            var universitiesData = JsonSerializer.Deserialize<JsonUniversitiesData>(
                jsonContent,
                options
            );

            if (universitiesData?.Universities == null || !universitiesData.Universities.Any())
            {
                return BadRequest("No valid university data found in the JSON file.");
            }

            foreach (var university in universitiesData.Universities)
            {
                var insertUnivesity = new Models.Universities
                {
                    UniversityId = university.UniId,
                    UniversityName = university.Name,
                    CreatedAt = DateTime.UtcNow,
                };

                await _supabase.From<Models.Universities>().Upsert(insertUnivesity);
                foreach (var subject in university.Subjects)
                {
                    var insertSubject = new Models.UniversityCourses
                    {
                        CourseId = subject.SubjectId,
                        UniversityId = university.UniId,
                        DegreeId = 1,
                        CourseName = subject.Name,
                        CreatedAt = DateTime.UtcNow,
                        price = int.Parse(subject.Price),
                    };

                    await _supabase.From<Models.UniversityCourses>().Upsert(insertSubject);
                }
            }
            return Ok("Universities data processed successfully.");
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
            [FromForm] string universityId,
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
