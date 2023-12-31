
using AuthFilterProj.Dtos;
using AuthFilterProj.Models;

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

        Task<Response<string>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<Response<string>> ResendOtpAsync(ResendOtpDto resendOtpDto);

        Task<Response<string>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<Response<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        User GetUserById(int id);

         Task<Response<string>> UploadProfilePicture(IFormFile file);
         
    }


}