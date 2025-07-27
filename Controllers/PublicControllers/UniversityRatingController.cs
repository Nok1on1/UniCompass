using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniCompass.DTOs.UniversityRatingDtos;

namespace UniCompass.Controllers.PublicControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityRatingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Supabase.Client _supabase;

        public UniversityRatingController(
            IMapper mapper,
            [FromKeyedServices("user")] Supabase.Client supabase
        )
        {
            _mapper = mapper;
            _supabase = supabase;
        }

        [HttpPost("RateUniversity")]
        public async Task<IActionResult> RateUniversity([FromForm] PostUniversityRating ratingDto)
        {
            if (ratingDto == null)
            {
                return BadRequest("Invalid rating data.");
            }

            var university = await _supabase
                .From<Models.Universities>()
                .Where(x => x.UniversityId == ratingDto.UniversityId)
                .Single();

            if (university == null)
                return NotFound("University not found.");

            var userId = Guid.Parse(_supabase.Auth.CurrentUser?.Id);

            if (userId == Guid.Empty)
                return Unauthorized("User not authenticated.");

            var rating = _mapper.Map<Models.UniversityRatings>(ratingDto);

            rating.UserId = userId;

            if (rating.RatingValue < 1 || rating.RatingValue > 5)
            {
                return BadRequest("Rating value must be between 1 and 5.");
            }

            var existingRating = await _supabase
                .From<Models.UniversityRatings>()
                .Where(x => x.UniversityId == ratingDto.UniversityId && x.UserId == userId)
                .Single();

            var result = await _supabase.From<Models.UniversityRatings>().Insert(rating);

            if (!result.ResponseMessage.IsSuccessStatusCode)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    result.ResponseMessage.ReasonPhrase
                );
            }

            return Ok("Rating submitted successfully.");
        }

        [HttpPut("UpdateUniversityRating")]
        public async Task<IActionResult> UpdateUniversityRating(
            [FromForm] PostUniversityRating ratingDto
        )
        {
            if (ratingDto == null)
            {
                return BadRequest("Invalid rating data.");
            }

            var university = await _supabase
                .From<Models.Universities>()
                .Where(x => x.UniversityId == ratingDto.UniversityId)
                .Single();

            if (university == null)
                return NotFound("University not found.");

            var userId = Guid.Parse(_supabase.Auth.CurrentUser?.Id);

            if (userId == Guid.Empty)
                return Unauthorized("User not authenticated.");

            var existingRating = await _supabase
                .From<Models.UniversityRatings>()
                .Where(x => x.UniversityId == ratingDto.UniversityId && x.UserId == userId)
                .Single();

            if (existingRating == null)
            {
                return NotFound("Rating not found for the user.");
            }

            var updatedRating = _mapper.Map<Models.UniversityRatings>(ratingDto);

            updatedRating.UserId = userId;
            updatedRating.RatingId = existingRating.RatingId;
            updatedRating.Likes = existingRating.Likes;

            if (updatedRating.RatingValue < 1 || updatedRating.RatingValue > 5)
            {
                return BadRequest("Rating value must be between 1 and 5.");
            }

            var result = await _supabase.From<Models.UniversityRatings>().Update(updatedRating);

            if (!result.ResponseMessage.IsSuccessStatusCode)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    result.ResponseMessage.ReasonPhrase
                );
            }

            return Ok("Rating updated successfully.");
        }
    }
}
