using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthFilterProj.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthFilterProj.Utils
{
public class TokenUtils
    {
        private readonly string _jwtSecretKey;

        public TokenUtils(string jwtSecretKey)
        {
            _jwtSecretKey = jwtSecretKey;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()), 
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Name),
                    // Add more claims here if necessary
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}