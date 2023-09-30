using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        //refresh token
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        

        //  public ClaimsPrincipal GetPrincipalFromToken(string token)
        // {
        //     var tokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidateAudience = false,
        //         ValidateIssuer = false,
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey)),
        //         ValidateLifetime = false
        //     };
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     SecurityToken securityToken;
        //     //principal
        //     var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        //     var jwtSecurityToken = securityToken as JwtSecurityToken;
        //     if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //         throw new SecurityTokenException("Invalid token passed");
        //     return principal;
        // }

     

        
    }
}