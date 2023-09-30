using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


    }

    
}