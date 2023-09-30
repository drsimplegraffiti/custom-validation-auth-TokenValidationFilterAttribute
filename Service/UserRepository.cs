using AuthFilterProj.Data;
using AuthFilterProj.Dtos;
using AuthFilterProj.Interface;
using AuthFilterProj.Models;
using AuthFilterProj.Utils;
using Microsoft.EntityFrameworkCore;

namespace AuthFilterProj.Service
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DataContext context, IConfiguration configuration, ILogger<UserRepository> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Response<ReadUserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password)
            };

            await _context.Users.AddAsync(user);
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

            // Verify the password
            if (!BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.Password))
            {
                // Password doesn't match, return an error response
                return new Response<LoginResponseDto>
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            // Password is correct, generate a token
            var tokenUtils = new TokenUtils(_configuration["JWT:SecretKey"] ?? string.Empty);
            var loginResponseDto = new LoginResponseDto
            {
                Token = tokenUtils.GenerateToken(user)
            };

            // Return a success response with the token
            return new Response<LoginResponseDto>
            {
                Success = true,
                Data = loginResponseDto,
                Message = "User logged in successfully!"
            };
        }


    }
}