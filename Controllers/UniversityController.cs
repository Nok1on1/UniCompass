using Microsoft.AspNetCore.Mvc;

namespace UniCompass.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityController : ControllerBase
    {
        private readonly Supabase.Client _supabase;

        public UniversityController(Supabase.Client supabase)
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
    }
}
