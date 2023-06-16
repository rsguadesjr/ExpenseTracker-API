using AutoMapper;
using ExpenseTracker.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class UserRoleRepository : BaseRepository<UserRole>
    {
        public UserRoleRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<UserRole> Update(UserRole entity)
        {
            var userRole = await _context.UserRoles.SingleAsync(x => x.Id == entity.Id);
            userRole.RoleId = entity.RoleId;

            return entity;
        }
    }
}
