

using System.IdentityModel.Tokens.Jwt;
using AuthFilterProj.Custom;
using AuthFilterProj.Data;
using AuthFilterProj.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthFilterProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class GoogleController : ControllerBase
    {
        private readonly ILogger<GoogleController> _logger;
        private readonly IConfiguration _configuration;

        private readonly DataContext    _context;

        public GoogleController(ILogger<GoogleController> logger, IConfiguration configuration, DataContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("signin-google")]
        [AllowAnonymous]
        public IActionResult SignInWithGoogle([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest("Invalid token");
                }

                // Decode and verify the Google token
                var payload = GoogleJsonWebSignature.ValidateAsync(token).Result;
                Console.WriteLine("Google token payload: " + payload);

                // You can access claims like email, name, etc. from the payload
                var email = payload.Email;
                var name = payload.Name;
                var pictureUrl = payload.Picture;
                var userId = payload.Subject;
                var issuer = payload.Issuer;
                var issuedAt = payload.IssuedAtTimeSeconds;
                var expiresAt = payload.ExpirationTimeSeconds;
                var audience = payload.Audience;
                var locale = payload.Locale;


                Console.WriteLine("Email: " + email);
                Console.WriteLine("Name: " + name);
                Console.WriteLine("Picture URL: " + pictureUrl);
                Console.WriteLine("User ID: " + userId);
                Console.WriteLine("Issuer: " + issuer);
                Console.WriteLine("Issued At: " + issuedAt);
                Console.WriteLine("Expires At: " + expiresAt);
                Console.WriteLine("Audience: " + audience);

                // Add your user to the database if it doesn't exist already
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                     GoogleEmail = email,
                        GoogleId = userId,
                        GoogleLocale = locale,
                        GoogleName = name,
                        GooglePictureUrl = pictureUrl,
                        Email = email,
                        Name = name,
                        ProfilePicture = pictureUrl,
                        IsVerified = true,
                        Role = "User",
                        Password = BCrypt.Net.BCrypt.HashPassword("12345678"),
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                }


                // Add your custom validation logic here if needed
                
                return Ok("Token is valid, User logged in successfully");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine("Token validation error: " + ex.ToString());
                return BadRequest($"Token validation failed: {ex.Message}");
            }
        }

    }
}
