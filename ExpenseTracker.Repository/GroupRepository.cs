using AutoMapper;
using ExpenseTracker.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseTracker.Repository
{
    public class GroupRepository : BaseRepository<Group>
    {
        public GroupRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<Group> Update(Group entity)
        {
            var group = await _context.Groups.SingleAsync(x => x.Id == entity.Id);
            group.Name = entity.Name;
            group.IsActive = entity.IsActive;

            return entity;
        }
    }
}
