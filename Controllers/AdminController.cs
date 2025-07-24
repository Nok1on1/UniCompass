using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniCompass.DTOs;

namespace UniCompass.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Supabase.Client _userClient;
        private readonly Cloudinary _cloudinary;

        public AdminController(
            IMapper mapper,
            [FromKeyedServices("admin")] Supabase.Client supabase,
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
    }
}
