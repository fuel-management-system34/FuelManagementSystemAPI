using AutoMapper;
using FuelManagementSystem.Application.DTOs.User;
using FuelManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Map from User to UserDto
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles.Select(ur => ur.Role.Name).ToList()));

            // Map from CreateUserDto to User
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            // Map from UpdateUserDto to User
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
        }
    }
}
