
using System.Security.Claims;
using AuthFilterProj.Custom;
using AuthFilterProj.Dtos;
using AuthFilterProj.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthFilterProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(TokenValidationFilterAttribute))] // Apply the token validation filter
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<IUserRepository> _logger;

        public UsersController(IUserRepository userRepository, ILogger<IUserRepository> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpPost("Register")]
        [SkipTokenValidation] // Skip token validation for this action

        public async Task<IActionResult> CreateUserAsync(CreateUserDto createUserDto)
        {
            var response = await _userRepository.CreateUserAsync(createUserDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            // Access user claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);

            if (userIdClaim != null && userEmailClaim != null)
            {
                var userId = userIdClaim.Value;
                var userEmail = userEmailClaim.Value;

                _logger.LogInformation($"User with ID: {userId} and Email: {userEmail} is requesting user information.");
            }
            var response = await _userRepository.GetUserAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var response = await _userRepository.GetUsersAsync();

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(int id, CreateUserDto createUserDto)
        {
            var response = await _userRepository.UpdateUserAsync(id, createUserDto);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var response = await _userRepository.DeleteUserAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("verify-otp")]
         [SkipTokenValidation]
        public async Task<IActionResult> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var response = await _userRepository.VerifyOtpAsync(verifyOtpDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtpAsync(ResendOtpDto resendOtpDto)
        {
            var response = await _userRepository.ResendOtpAsync(resendOtpDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}