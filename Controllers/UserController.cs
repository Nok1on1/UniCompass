using AutoMapper;
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
        private readonly Supabase.Client _adminClient;

        public UserController(
            IMapper mapper,
            [FromKeyedServices("user")] Supabase.Client supabase,
            [FromKeyedServices("admin")] Supabase.Client adminClient
        )
        {
            _mapper = mapper;
            _userClient = supabase;
            _adminClient = adminClient;
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
    }
}
