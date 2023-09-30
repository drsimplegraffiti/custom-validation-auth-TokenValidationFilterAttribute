
using AuthFilterProj.Dtos;

namespace AuthFilterProj.Interface
{
    public interface IUserRepository
    {
        Task<Response<ReadUserDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<ReadUserDto>> GetUserAsync(int id);
        Task<Response<ReadUserDto>> UpdateUserAsync(int id, CreateUserDto createUserDto);
        Task<Response<ReadUserDto>> DeleteUserAsync(int id);
        Task<Response<IEnumerable<ReadUserDto>>> GetUsersAsync();
        Task<Response<LoginResponseDto>> LoginAsync(LoginDto loginRequestDto); // Add this method
        bool GetUserByEmail(string email);
        Task<Response<LoginResponseDto>> RefreshTokenAsync(string refreshToken);

       //verify otp
        Task<Response<string>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<Response<string>> ResendOtpAsync(ResendOtpDto resendOtpDto);
    }

    public class ResendOtpDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}