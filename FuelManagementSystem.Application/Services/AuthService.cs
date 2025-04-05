using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Repositiries;
using FuelManagementSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;





namespace FuelManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserActivityLogRepository _userActivityLogRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IUserActivityLogRepository userActivityLogRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userActivityLogRepository = userActivityLogRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool Success, string Token, string RefreshToken)> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null || !user.IsActive)
                {
                    return (false, null, null);
                }

                // to be implement

                // Verify password
                //if (!VerifyPassword(password, user.PasswordHash))
                //{
                //    // Log failed login attempt
                //    await _userActivityLogRepository.AddAsync(new UserActivityLog
                //    {
                //        UserId = user.UserId,
                //        ActivityType = "LoginFailed",
                //        Description = "Failed login attempt - incorrect password",
                //        IPAddress = "0.0.0.0" // This would normally come from the request
                //    });

                //    return (false, null, null);
                //}

                // Update last login time
                user.LastLogin = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                // Log successful login
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = user.UserId,
                    ActivityType = "Login",
                    Description = "User logged in successfully",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                //get user roles
                var currentRoles = await _userRoleRepository.GetByUserIdAsync(user.UserId);

                // Generate tokens
                var token = GenerateJwtToken(user, currentRoles);
                var refreshToken = GenerateRefreshToken();

                // In a real application, store the refresh token securely in the database

                return (true, token, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login: {Username}", username);
                return (false, null, null);
            }
        }

        public async Task<(bool Success, string Token, string RefreshToken)> GoogleLoginAsync(string email, string googleToken)
        {
            try
            {
                // In a real application, validate the Google token
                // For this example, we're assuming the token is valid

                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    // User is not registered in our system
                    return (false, null, null);
                }

                if (!user.IsActive)
                {
                    return (false, null, null);
                }

                // Update last login time
                user.LastLogin = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                // Log successful login via Google
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = user.UserId,
                    ActivityType = "GoogleLogin",
                    Description = "User logged in successfully using Google",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                var currentRoles = await _userRoleRepository.GetByUserIdAsync(user.UserId);

                // Generate tokens
                var token = GenerateJwtToken(user, currentRoles);
                var refreshToken = GenerateRefreshToken();

                // In a real application, store the refresh token securely in the database

                return (true, token, refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login: {Email}", email);
                return (false, null, null);
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password, string firstName, string lastName)
        {
            try
            {
                // Check if user already exists
                if (await _userRepository.ExistsByEmailAsync(email) ||
                    await _userRepository.ExistsByUsernameAsync(username))
                {
                    return false;
                }

                // Create new user
                var user = new User
                {
                    Username = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var createdUser = await _userRepository.AddAsync(user);

                // In a real application, we might assign a default "User" role
                // For this example, let's assume there's a role with ID 3 (Cashier)
                await _userRoleRepository.AddAsync(new UserRole
                {
                    UserId = createdUser.UserId,
                    RoleId = 3
                });

                // Log user registration
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = createdUser.UserId,
                    ActivityType = "Registration",
                    Description = "User registered successfully",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration: {Username}", username);
                return false;
            }
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            // This is a simplified token validation
            // In a real application, use proper JWT validation

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<(bool Success, string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            // In a real application, validate the refresh token against stored tokens
            // For this example, we're generating a new JWT token

            try
            {
                // This is where you would validate the refresh token from your database
                // For this example, we're assuming it's valid

                // Generate a new token (you would get the user from the refresh token)
                // This is simplified - in reality you need to get the user from the stored refresh token
                var user =  _userRepository.GetAllAsync().Result.First();
                var currentRoles = _userRoleRepository.GetByUserIdAsync(user.UserId);

                var newToken = GenerateJwtToken(user, currentRoles);
                var newRefreshToken = GenerateRefreshToken();

                return Task.FromResult((true, newToken, newRefreshToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Task.FromResult((false, string.Empty, string.Empty));
            }
        }

        public async Task<bool> LogoutAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null)
                {
                    return false;
                }

                // In a real application, invalidate refresh tokens

                // Log logout
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = user.UserId,
                    ActivityType = "Logout",
                    Description = "User logged out",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout: {Username}", username);
                return false;
            }
        }

        // Helper methods
        private string GenerateJwtToken(User user, dynamic userRoles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName ?? string.Empty),
                new Claim("LastName", user.LastName ?? string.Empty)
            };


            // Add roles to claims
            foreach (var userRole in userRoles)
            {
                var role = _roleRepository.GetByIdAsync(userRole.RoleId).Result;
                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string HashPassword(string password)
        {
            // In a real application, use a secure password hashing library like BCrypt
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
