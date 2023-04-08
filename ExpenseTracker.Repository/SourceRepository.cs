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
    public class SourceRepository : BaseRepository<Source>
    {
        public SourceRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<Source> Update(Source entity)
        {
            var source = await _context.Sources.SingleAsync(x => x.Id == entity.Id);
            source.Name = entity.Name;
            source.Description = entity.Description;

            return entity;
        }
    }
}
