using Microsoft.AspNetCore.Mvc;

namespace UniCompass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly Supabase.Client _supabase;

        public UniversityController([FromKeyedServices("user")] Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        [HttpGet("GetAllUniversities")]
        public async Task<IActionResult> GetAllUniversities()
        {
            var response = await _supabase.From<Models.Universities>().Get();

            if (response.Models == null || !response.Models.Any())
            {
                return NotFound("No universities found.");
            }

            var universities = response.Models;

            return Ok(universities);
        }

        [HttpGet("GetUniversityById")]
        public async Task<IActionResult> GetUniversityById(string universityId)
        {
            var response = await _supabase
                .From<Models.Universities>()
                .Where(x => x.UniversityId == universityId)
                .Single();

            if (response == null)
            {
                return NotFound("University not found.");
            }

            return Ok(response);
        }
    }
}
