
using AuthFilterProj.Dtos;
using AuthFilterProj.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthFilterProj.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AuthControlller : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<IUserRepository> _logger;
        public AuthControlller(IUserRepository userRepository, ILogger<IUserRepository> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var response = await _userRepository.LoginAsync(loginDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            _logger.LogInformation($"User with Email: {loginDto.Email} is requesting user information.");
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var response = await _userRepository.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (response.Success)
            {
                return Ok(response.Data);
            }

            return BadRequest(response.Message);
        }

    }


}