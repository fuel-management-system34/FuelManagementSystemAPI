using AutoMapper;
using FuelManagementSystem.Core.Entities;
using FuelManagementSystem.Core.Interfaces.Repositiries;
using FuelManagementSystem.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserActivityLogRepository _userActivityLogRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IUserActivityLogRepository userActivityLogRepository,
            ILogger<UserService> logger,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userActivityLogRepository = userActivityLogRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _userRepository.GetUsersByRoleAsync(roleId);
        }

        public async Task<User> CreateUserAsync(User user, string password, List<int> roleIds)
        {
            try
            {
                // Check if user with same email or username already exists
                if (await _userRepository.ExistsByEmailAsync(user.Email))
                {
                    throw new Exception("User with the same email already exists.");
                }

                if (await _userRepository.ExistsByUsernameAsync(user.Username))
                {
                    throw new Exception("User with the same username already exists.");
                }

                // Hash the password
                user.PasswordHash = HashPassword(password);

                // Create the user
                var createdUser = await _userRepository.AddAsync(user);

                // Assign roles to the user
                if (roleIds != null && roleIds.Any())
                {
                    foreach (var roleId in roleIds)
                    {
                        if (await _roleRepository.ExistsAsync(roleId))
                        {
                            await _userRoleRepository.AddAsync(new UserRole
                            {
                                UserId = createdUser.UserId,
                                RoleId = roleId
                            });
                        }
                    }
                }

                // Log user creation
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = createdUser.UserId,
                    ActivityType = "UserCreated",
                    Description = "User account created",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", user.Username);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(User user, List<int> roleIds)
        {
            try
            {
                // Check if user exists
                var existingUser = await _userRepository.GetByIdAsync(user.UserId);
                if (existingUser == null)
                {
                    return false;
                }

                // Update user properties
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.ProfilePicture = user.ProfilePicture;
                existingUser.PreferredLanguage = user.PreferredLanguage;
                existingUser.IsActive = user.IsActive;
                existingUser.UpdatedAt = DateTime.Now;

                // Only update email/username if they've changed and don't conflict with existing users
                if (existingUser.Email != user.Email)
                {
                    if (await _userRepository.ExistsByEmailAsync(user.Email))
                    {
                        throw new Exception("User with the same email already exists.");
                    }
                    existingUser.Email = user.Email;
                }

                if (existingUser.Username != user.Username)
                {
                    if (await _userRepository.ExistsByUsernameAsync(user.Username))
                    {
                        throw new Exception("User with the same username already exists.");
                    }
                    existingUser.Username = user.Username;
                }

                // Update the user in the database
                await _userRepository.UpdateAsync(existingUser);

                // Update user roles if provided
                if (roleIds != null)
                {
                    // Get current roles
                    var currentRoles = await _userRoleRepository.GetByUserIdAsync(user.UserId);
                    var currentRoleIds = currentRoles.Select(r => r.RoleId).ToList();

                    // Add new roles
                    foreach (var roleId in roleIds.Where(r => !currentRoleIds.Contains(r)))
                    {
                        if (await _roleRepository.ExistsAsync(roleId))
                        {
                            await _userRoleRepository.AddAsync(new UserRole
                            {
                                UserId = user.UserId,
                                RoleId = roleId
                            });
                        }
                    }

                    // Remove roles that aren't in the new list
                    foreach (var roleId in currentRoleIds.Where(r => !roleIds.Contains(r)))
                    {
                        await _userRoleRepository.DeleteByUserAndRoleAsync(user.UserId, roleId);
                    }
                }

                // Log user update
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = user.UserId,
                    ActivityType = "UserUpdated",
                    Description = "User account updated",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", user.UserId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                // Check if user exists
                if (!(await _userRepository.ExistsAsync(id)))
                {
                    return false;
                }

                // Delete user
                await _userRepository.DeleteAsync(id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Verify current password
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                // Update with new password
                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                // Log password change
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = userId,
                    ActivityType = "PasswordChanged",
                    Description = "User changed password",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Update with new password
                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                // Log password reset
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = userId,
                    ActivityType = "PasswordReset",
                    Description = "User password was reset",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> AssignRoleAsync(int userId, int roleId)
        {
            try
            {
                if (!(await _userRepository.ExistsAsync(userId)) ||
                    !(await _roleRepository.ExistsAsync(roleId)))
                {
                    return false;
                }

                if (await _userRoleRepository.ExistsAsync(userId, roleId))
                {
                    // Role already assigned
                    return true;
                }

                await _userRoleRepository.AddAsync(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });

                // Log role assignment
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = userId,
                    ActivityType = "RoleAssigned",
                    Description = $"Role with ID {roleId} assigned to user",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                throw;
            }
        }

        public async Task<bool> RemoveRoleAsync(int userId, int roleId)
        {
            try
            {
                if (!(await _userRoleRepository.ExistsAsync(userId, roleId)))
                {
                    return false;
                }

                await _userRoleRepository.DeleteByUserAndRoleAsync(userId, roleId);

                // Log role removal
                await _userActivityLogRepository.AddAsync(new UserActivityLog
                {
                    UserId = userId,
                    ActivityType = "RoleRemoved",
                    Description = $"Role with ID {roleId} removed from user",
                    IPAddress = "0.0.0.0" // This would normally come from the request
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
            var roles = new List<Role>();

            foreach (var userRole in userRoles)
            {
                var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId);
            foreach (var userRole in userRoles)
            {
                var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                if (role != null && role.Name == roleName)
                {
                    return true;
                }
            }

            return false;
        }

        // Helper methods for password hashing
        private string HashPassword(string password)
        {
            // In a real application, use a secure password hashing library like BCrypt
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            // In a real application, use a secure password verification method
            var newHash = HashPassword(password);
            return newHash == hash;
        }
    }
}
