using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.DTOs.Role
{
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateRoleDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }

    public class UpdateRoleDto
    {
        [Required]
        public int RoleId { get; set; }

        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }


    public class LoginDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class GoogleLoginDto
    {
        [Required]
        public string GoogleToken { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
