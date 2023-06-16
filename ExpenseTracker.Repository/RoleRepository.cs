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
    public class RoleRepository : BaseRepository<Role>
    {
        public RoleRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<Role> Update(Role entity)
        {
            var role = await _context.Roles.SingleAsync(x => x.Id == entity.Id);
            role.Name = entity.Name;
            role.IsDefault = entity.IsDefault;
            role.IsActive = entity.IsActive;

            return entity;
        }
    }
}
