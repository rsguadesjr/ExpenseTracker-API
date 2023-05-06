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
    public class ExpenseTagRepository : BaseRepository<ExpenseTag>
    {
        public ExpenseTagRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // No implementation since we don't update, we only add or delete for this entity
        public override async Task<ExpenseTag> Update(ExpenseTag entity)
        {
            throw new NotImplementedException();
        }
    }
}
