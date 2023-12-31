using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthFilterProj.Data;
using AuthFilterProj.Dtos;
using AuthFilterProj.Interface;
using AuthFilterProj.Models;
using AuthFilterProj.Templates;
using AuthFilterProj.Utils;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthFilterProj.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly Cloudinary _cloudinary;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        private readonly ILogger<UserRepository> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserRepository(
            DataContext context,
            IConfiguration configuration,
            ILogger<UserRepository> logger,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor = null!,
            Cloudinary cloudinary = null!)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _cloudinary = CloudinaryHelper.CreateCloudinaryInstance(_configuration);
        }

        public async Task<Response<ReadUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if the email is already registered
            var userExists = GetUserByEmail(createUserDto.Email);
            if (userExists)
            {
                return new Response<ReadUserDto>
                {
                    Success = false,
                    Message = "Email already registered."
                };
            }

            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            //otp
            var otp = new Otp
            {
                Email = user.Email,
                OtpCode = new Random().Next(1000, 9999).ToString(),
                ExpirationDateTime = DateTime.Now.AddMinutes(5)
            };

            await _context.Otps.AddAsync(otp);
            await _context.SaveChangesAsync();



            var readUserDto = new ReadUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            var response = new Response<ReadUserDto>
            {
                Data = readUserDto,
                Message = "User created successfully!"
            };

            _logger.LogInformation("User created successfully!");

            string emailContent = EmailTemplates.GetVerificationEmail(user.Name, otp.OtpCode);
            _emailService.Send(user.Email, "Verify your email", emailContent);


            return response;
        }

        public async Task<Response<ReadUserDto>> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return new Response<ReadUserDto>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var readUserDto = new ReadUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            var response = new Response<ReadUserDto>
            {
                Data = readUserDto,
                Message = "User retrieved successfully!"
            };

            return response;
        }

        public async Task<Response<IEnumerable<ReadUserDto>>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();

            var readUsersDto = users.Select(u => new ReadUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            });

            var response = new Response<IEnumerable<ReadUserDto>>
            {
                Data = readUsersDto,
                Message = "Users retrieved successfully!"
            };

            return response;
        }

        public async Task<Response<ReadUserDto>> UpdateUserAsync(int id, CreateUserDto createUserDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return new Response<ReadUserDto>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            user.Name = createUserDto.Name;
            user.Email = createUserDto.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var readUserDto = new ReadUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            var response = new Response<ReadUserDto>
            {
                Data = readUserDto,
                Message = "User updated successfully!"
            };

            return response;
        }

        public async Task<Response<ReadUserDto>> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return new Response<ReadUserDto>
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            var readUserDto = new ReadUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            var response = new Response<ReadUserDto>
            {
                Data = readUserDto,
                Message = "User deleted successfully!"
            };

            return response;
        }

        public async Task<Response<LoginResponseDto>> LoginAsync(LoginDto loginRequestDto)
        {
            try
            {
                // Find the user by email in your database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequestDto.Email);

                if (user == null)
                {
                    // User not found, return an error response
                    return new Response<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Invalid credentials."
                    };
                }

                // Check if the user's email is verified
                if (!user.IsVerified!.Value)
                {
                    // User's email is not verified, return an error response
                    return new Response<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Email not verified."
                    };
                }

                  if (user.NoOfLoginTries >= 3)
                {
                    // User's email is not verified, return an error response
                    return new Response<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Account locked, contact admin."
                    };
                }

                // Verify the password
                if (!BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.Password))
                {
                    // update the number of failed login attempts
                    user.NoOfLoginTries += 1;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    
                    // Password doesn't match, return an error response
                    return new Response<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Invalid credentials."
                    };
                }

                // reset the number of failed login attempts
                user.NoOfLoginTries = 0;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();


              

                // Password is correct, generate a token
                var tokenUtils = new TokenUtils(_configuration["JWT:SecretKey"] ?? string.Empty);
                var loginResponseDto = new LoginResponseDto
                {
                    Token = tokenUtils.GenerateToken(user),
                    RefreshToken = tokenUtils.GenerateRefreshToken()
                };

                // get ip address of user
                var ipAddress = _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString();
                _logger.LogInformation($"ipAddress: {ipAddress}");

                // get user agent
                var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
                _logger.LogInformation($"userAgent: {userAgent}");

                   // send email if user logs in from a new device
                if (user.UserAgent != userAgent || user.IpAddress != ipAddress)
                {
                    string emailContent = EmailTemplates.GetNewDeviceLoginEmail(user.Name, user.IpAddress, user.UserAgent);
                    _emailService.Send(user.Email, "New device login", emailContent);
                }

                user.IpAddress = ipAddress;
                user.UserAgent = userAgent;

             

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // remove old refresh token and token
                var OldserToken = await _context.UserTokens.FirstOrDefaultAsync(u => u.UserId == user.Id);
                _logger.LogInformation($"OldserToken: {OldserToken}");
                if (OldserToken != null)
                {
                    _context.UserTokens.Remove(OldserToken);
                }



                // Save the token and the refresh token in your database
                var userToken = new UserToken
                {
                    Token = loginResponseDto.Token,
                    RefreshToken = loginResponseDto.RefreshToken,
                    UserId = user.Id
                };


                await _context.UserTokens.AddAsync(userToken);
                await _context.SaveChangesAsync();

                // add token to response cookie
                _httpContextAccessor.HttpContext!.Response.Cookies.Append("token", loginResponseDto.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // add refresh token to response cookie
                _httpContextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", loginResponseDto.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Return a success response with the token
                return new Response<LoginResponseDto>
                {
                    Success = true,
                    Data = loginResponseDto,
                    Message = "User logged in successfully!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new Response<LoginResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public bool GetUserByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }
            return true;
        }


        public async Task<Response<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Find the user token associated with the provided refresh token
                var userToken = await _context.UserTokens.FirstOrDefaultAsync(ut => ut.RefreshToken == refreshToken);

                if (userToken == null)
                {
                    // Refresh token not found, return an error response
                    return new Response<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Invalid refresh token."
                    };
                }

                // // Verify if the refresh token has expired (you can set an expiration time for refresh tokens)
                if (userToken.ExpirationDateTime <= DateTime.UtcNow)
                {
                    // Refresh token has expired, remove it from the database and return an error response
                    _context.UserTokens.Remove(userToken);
                    await _context.SaveChangesAsync();
                    return new Response<LoginResponseDto>
                    {
                        Success = false,
                        Message = "Refresh token has expired."
                    };
                }

                // Generate a new access token
                var tokenUtils = new TokenUtils(_configuration["JWT:SecretKey"] ?? string.Empty);
                var user = await _context.Users.FindAsync(userToken.UserId);
                var loginResponseDto = new LoginResponseDto
                {
                    Token = tokenUtils.GenerateToken(user ?? throw new InvalidOperationException()),
                    RefreshToken = tokenUtils.GenerateRefreshToken()
                };

                // Update the user's token with the new refresh token
                userToken.RefreshToken = loginResponseDto.RefreshToken;
                _context.UserTokens.Update(userToken);
                await _context.SaveChangesAsync();

                // Return a success response with the new access token and refresh token
                return new Response<LoginResponseDto>
                {
                    Success = true,
                    Data = loginResponseDto,
                    Message = "Token refreshed successfully!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new Response<LoginResponseDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public Task<Response<string>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            //check if email exists
            var user = _context.Users.FirstOrDefault(u => u.Email == verifyOtpDto.Email);
            //return Error
            if (user == null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "Email not found."
                });
            }

            //check if otp exists
            var otp = _context.Otps.FirstOrDefault(o => o.Email == verifyOtpDto.Email && o.OtpCode == verifyOtpDto.Otp);
            //return Error
            if (otp == null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "Invalid OTP."
                });
            }

            //check if otp expired
            if (otp.ExpirationDateTime < DateTime.Now)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "OTP expired."
                });
            }

            //update user
            user.IsVerified = true;
            _context.Users.Update(user);
            _context.SaveChanges();

            // delete otp from db
            _context.Otps.Remove(otp);
            _context.SaveChanges();

            //return success
            return Task.FromResult(new Response<string>
            {
                Success = true,
                Message = "OTP verified successfully."
            });
        }

        public Task<Response<string>> ResendOtpAsync(ResendOtpDto resendOtpDto)
        {
            //check if email exists
            var user = _context.Users.FirstOrDefault(u => u.Email == resendOtpDto.Email);
            //return Error
            if (user == null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "Email not found."
                });
            }

            //check if otp exists
            var otp = _context.Otps.FirstOrDefault(o => o.Email == resendOtpDto.Email);
            // return Error
            if (otp != null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "OTP already sent."
                });
            }

            //otp
            var newOtp = new Otp
            {
                Email = user.Email,
                OtpCode = new Random().Next(1000, 9999).ToString(),
                ExpirationDateTime = DateTime.Now.AddMinutes(5)
            };

            _context.Otps.Add(newOtp);
            _context.SaveChanges();

            //return success
            return Task.FromResult(new Response<string>
            {
                Success = true,
                Message = "OTP sent successfully."
            });
        }

        public Task<Response<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            // check if email exists
            var user = _context.Users.FirstOrDefault(u => u.Email == forgotPasswordDto.Email);
            // return Error
            if (user == null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "Email not found."
                });
            }

            // otp
            var otp = new Otp
            {
                Email = user.Email,
                OtpCode = new Random().Next(1000, 9999).ToString(),
                ExpirationDateTime = DateTime.Now.AddMinutes(5)
            };

            _context.Otps.Add(otp);
            _context.SaveChanges();

            // send email
            string emailContent = EmailTemplates.GetForgotPasswordEmail(user.Name, otp.OtpCode);
            _emailService.Send(user.Email, "Reset your password", emailContent);

            // return success
            return Task.FromResult(new Response<string>
            {
                Success = true,
                Message = "OTP sent successfully."
            });
        }

        public Task<Response<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // check if email exists
            var user = _context.Users.FirstOrDefault(u => u.Email == resetPasswordDto.Email);
            // return Error
            if (user == null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "Email not found."
                });
            }

            // check if otp exists
            var otp = _context.Otps.FirstOrDefault(o => o.Email == resetPasswordDto.Email && o.OtpCode == resetPasswordDto.Otp);
            // return Error
            if (otp == null)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "Invalid OTP."
                });
            }

            // check if otp expired
            if (otp.ExpirationDateTime < DateTime.Now)
            {
                return Task.FromResult(new Response<string>
                {
                    Success = false,
                    Message = "OTP expired."
                });
            }

            // update user
            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
            _context.Users.Update(user);
            _context.SaveChanges();

            // delete otp from db
            _context.Otps.Remove(otp);
            _context.SaveChanges();

            // return success
            return Task.FromResult(new Response<string>
            {
                Success = true,
                Message = "Password reset successfully."
            });
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id) ?? throw new Exception("User not found");
        }

       public Task<Response<string>> UploadProfilePicture(IFormFile file)
{
    var principal = _httpContextAccessor.HttpContext!.User;
    if (principal == null)
    {
        return Task.FromResult(new Response<string>
        {
            Success = false,
            Message = "User not found"
        });
    }

    var userIdClaim = principal.Identity!.Name;

    if (string.IsNullOrEmpty(userIdClaim))
    {
        return Task.FromResult(new Response<string>
        {
            Success = false,
            Message = "User not found"
        });
    }

    var user = GetUserById(int.Parse(userIdClaim));
    if (user == null)
    {
        return Task.FromResult(new Response<string>
        {
            Success = false,
            Message = "User not found"
        });
    }
    _logger.LogInformation($"User: {user.Id}");

    var uploadResult = new ImageUploadResult();

    // Check if the user has an existing profile picture
    if (!string.IsNullOrEmpty(user.PublicId))
    {
        // Delete the existing image from Cloudinary
        var deleteParams = new DeletionParams(user.PublicId);
        var deletionResult = _cloudinary.Destroy(deleteParams);
        if (deletionResult.Result == "ok")
        {
            _logger.LogInformation($"Deleted old image with PublicId: {user.PublicId}");
        }
        else
        {
            _logger.LogError($"Failed to delete old image with PublicId: {user.PublicId}");
        }
    }

    if (file.Length > 0)
    {
        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };
        uploadResult = _cloudinary.Upload(uploadParams);
    }

    user.UpdatedAt = DateTime.Now;
    user.PublicId = uploadResult.PublicId;
    user.ProfilePicture = uploadResult.Url.ToString();
    _context.Users.Update(user);
    _context.SaveChanges();

    return Task.FromResult(new Response<string>
    {
        Success = true,
        Message = "Profile picture updated successfully",
        Data = uploadResult.Url.ToString()
    });
}


    }
}