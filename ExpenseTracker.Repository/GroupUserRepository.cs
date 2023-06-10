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
    public class GroupUserRepository : BaseRepository<GroupUser>
    {
        public GroupUserRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<GroupUser> Update(GroupUser entity)
        {
            var groupUser = await _context.GroupUsers.SingleAsync(x => x.Id == entity.Id);
            groupUser.IsActive = entity.IsActive;
            groupUser.IsAccepted = entity.IsAccepted;
            groupUser.AcceptedDate = entity.AcceptedDate;

            return entity;
        }
    }
}
