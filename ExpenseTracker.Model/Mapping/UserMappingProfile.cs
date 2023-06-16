using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserVM>();
            CreateMap<User, UserResponseModel>()
                .ForMember(x => x.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(s => new RoleResponseModel
                {
                    Id = s.RoleId,
                    Name = s.Role.Name,
                    IsActive = s.Role.IsActive
                })));
        }
    }
}
