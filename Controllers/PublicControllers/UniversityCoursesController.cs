using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UniCompass.Controllers.PublicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityCoursesController : ControllerBase
    {
        private readonly Supabase.Client _supabase;

        public UniversityCoursesController([FromKeyedServices("user")] Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        [HttpGet("GetCoursesByUniversityId")]
        public async Task<IActionResult> GetCoursesByUniversityId(string universityId)
        {
            if (string.IsNullOrEmpty(universityId))
            {
                return BadRequest("University ID is required.");
            }

            var response = await _supabase
                .From<Models.UniversityCourses>()
                .Where(x => x.UniversityId == universityId)
                .Get();

            if (response.Models == null || !response.Models.Any())
            {
                return NotFound("No courses found for the specified university.");
            }

            return Ok(response.Models);
        }
    }
}
