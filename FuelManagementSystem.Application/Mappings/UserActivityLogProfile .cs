using AutoMapper;
using FuelManagementSystem.Application.DTOs.UserActivityLog;
using FuelManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelManagementSystem.Application.Mappings
{
    public class UserActivityLogProfile : Profile
    {
        public UserActivityLogProfile()
        {
            // Map from UserActivityLog to UserActivityLogDto
            CreateMap<UserActivityLog, UserActivityLogDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));
        }
    }
}
