using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, string RefreshToken)> LoginAsync(string username, string password);
        Task<(bool Success, string Token, string RefreshToken)> GoogleLoginAsync(string email, string googleToken);
        Task<bool> RegisterAsync(string username, string email, string password, string firstName, string lastName);
        Task<bool> ValidateTokenAsync(string token);
        Task<(bool Success, string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string username);
    }
}
