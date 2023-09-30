
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

        //refresh token to create new token
        Task<Response<LoginResponseDto>> RefreshTokenAsync(string refreshToken);



    }


}