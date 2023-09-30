// using System.IdentityModel.Tokens.Jwt;
// using System.Text;
// using AuthFilterProj.Dtos;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using Microsoft.IdentityModel.Tokens;

// namespace AuthFilterProj.Custom
// {
//     public class TokenValidationFilterAttribute : Attribute, IActionFilter
//     {
//         //logger
//         private readonly ILogger<TokenValidationFilterAttribute> _logger;
//         private readonly IConfiguration _configuration;

//         public TokenValidationFilterAttribute(ILogger<TokenValidationFilterAttribute> logger, IConfiguration configuration)
//         {
//             _logger = logger;
//             _configuration = configuration;
//         }

//         public void OnActionExecuted(ActionExecutedContext context)
//         {
//             _logger.LogInformation("OnActionExecuted==================== In action filter for jwt token validation");
//         }

//         public void OnActionExecuting(ActionExecutingContext context)
//         {
//             var skipTokenValidation = context
//                 .ActionDescriptor?
//                 .EndpointMetadata
//                 .Any(em => em is SkipTokenValidationAttribute) == true;

//             if (skipTokenValidation)
//             {
//                 _logger.LogInformation("OnActionExecuting==================== Skip token validation");
//                 return; // Skip token validation if the custom attribute is present
//             }

//             var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

//             if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
//             {
//                 context.Result = new ObjectResult(new ErrorResponse
//                 {
//                     Message = "Missing token, Please provide a valid token",
//                     Error = "Unauthorized",
//                     Description = "Missing token, Please provide a valid token",
//                     Success = false

//                 })
//                 {
//                     StatusCode = 401
//                 };
//                 return;
//             }

//             var token = authHeader.Substring("Bearer ".Length);
//             var tokenHandler = new JwtSecurityTokenHandler();
//             var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? string.Empty);

//             try
//             {
//                 var tokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuerSigningKey = true,
//                     IssuerSigningKey = new SymmetricSecurityKey(key),
//                     ValidateIssuer = false, // Set to true if you want to validate the issuer
//                     ValidateAudience = false, // Set to true if you want to validate the audience
//                     ValidateLifetime = true // Set to true if you want to validate token expiration
//                 };

//                 var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
//                 context.HttpContext.User = principal; // this is like req.user in Express.js
//             }
//             catch (ArgumentException ex)
//             {
//                 // context.Result = new UnauthorizedResult();
//                 context.Result = new ObjectResult(new ErrorResponse
//                 {
//                     Message = "Invalid token format",
//                     Error = "Unauthorized",
//                     Description = ex.Message, // Include the exception message in the description
//                     Success = false

//                 })
//                 {
//                     StatusCode = 401
//                 };
//             }
//             catch (SecurityTokenException)
//             {
//                 // context.Result = new UnauthorizedResult();
//                 context.Result = new ObjectResult(new ErrorResponse
//                 {
//                     Message = "Unauthorized",
//                     Success = false,
//                     Error = "Unauthorized",
//                     Description = "Unauthorized"
//                 })
//                 {
//                     StatusCode = 401
//                 };
//             } 
//             catch (Exception ex) // capture the exception thrown when decoding an invalid token
//             {
//                 // context.Result = new UnauthorizedResult();
//                 context.Result = new ObjectResult(new ErrorResponse
//                 {
//                     Message = "Unauthorized",
//                     Success = false,
//                     Error = "Unauthorized",
//                     Description = ex.Message
//                 })
//                 {
//                     StatusCode = 401
//                 };
//             }
//         }
//     }
// }


// [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
// public sealed class SkipTokenValidationAttribute : Attribute
// {
// }

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AuthFilterProj.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System;

namespace AuthFilterProj.Custom
{
    public class TokenValidationFilterAttribute : Attribute, IActionFilter
    {
        private readonly ILogger<TokenValidationFilterAttribute> _logger;
        private readonly IConfiguration _configuration;

        public TokenValidationFilterAttribute(ILogger<TokenValidationFilterAttribute> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("OnActionExecuted==================== In action filter for JWT token validation");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var skipTokenValidation = context.ActionDescriptor?.EndpointMetadata
                .Any(em => em is SkipTokenValidationAttribute) == true;

            if (skipTokenValidation)
            {
                _logger.LogInformation("OnActionExecuting==================== Skip token validation");
                return; // Skip token validation if the custom attribute is present
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new ObjectResult(new ErrorResponse
                {
                    Message = "Missing token, please provide a valid token",
                    Error = "Unauthorized",
                    Description = "Missing token, please provide a valid token",
                    Success = false
                })
                {
                    StatusCode = 401
                };
                return;
            }

            var token = authHeader.Substring("Bearer ".Length);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? string.Empty);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Set to true if you want to validate the issuer
                    ValidateAudience = false, // Set to true if you want to validate the audience
                    ValidateLifetime = true // Set to true if you want to validate token expiration
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

                // Extract the UserId claim from the token
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Add the UserId to the request context so that it's available in the controller
                    context.HttpContext.Items["UserId"] = userId;

                    // Add the UserId to the request context so that it's available in the controller
                    // context.HttpContext.Items["UserEmail"] = principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                    // Add the UserId to the request context so that it's available in the controller
                    // context.HttpContext.Items["UserRole"] = principal.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                    // Add the UserId to the request context so that it's available in the controller
                    // context.HttpContext.Items["UserName"] = principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                }

                context.HttpContext.User = principal; // this is like req.user in Express.js
            }
            catch (ArgumentException ex)
            {
                context.Result = new ObjectResult(new ErrorResponse
                {
                    Message = "Invalid token format",
                    Error = "Unauthorized",
                    Description = ex.Message, // Include the exception message in the description
                    Success = false
                })
                {
                    StatusCode = 401
                };
            }
            catch (SecurityTokenException)
            {
                context.Result = new ObjectResult(new ErrorResponse
                {
                    Message = "Unauthorized",
                    Success = false,
                    Error = "Unauthorized",
                    Description = "Unauthorized"
                })
                {
                    StatusCode = 401
                };
            }
            catch (Exception ex) // capture the exception thrown when decoding an invalid token
            {
                context.Result = new ObjectResult(new ErrorResponse
                {
                    Message = "Unauthorized",
                    Success = false,
                    Error = "Unauthorized",
                    Description = ex.Message
                })
                {
                    StatusCode = 401
                };
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class SkipTokenValidationAttribute : Attribute
{
}
