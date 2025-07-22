using BrainHope.Services.DTO.Authentication.SingUp;
using CleanArchitecture.DataAccess.Models;
using CleanArchitecture.Services.DTOs.Responses;

namespace CleanArchitecture.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<TokenType>> RegisterUserAsync(RegisterUser registerUser);

        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
    }
}
