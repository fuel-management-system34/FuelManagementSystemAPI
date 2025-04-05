using AutoMapper;
using FuelManagementSystem.Application.DTOs.Role;
using FuelManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            // Map from Role to RoleDto
            CreateMap<Role, RoleDto>();

            // Map from CreateRoleDto to Role
            CreateMap<CreateRoleDto, Role>();

            // Map from UpdateRoleDto to Role
            CreateMap<UpdateRoleDto, Role>();
        }
    }
}
