using AutoMapper;
using ExpenseTracker.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class TagRepository : BaseRepository<Tag>
    {
        public TagRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        // No implementation since we don't update, we only add or delete for this entity
        public override async Task<Tag> Update(Tag entity)
        {
            throw new NotImplementedException();
        }
    }
}
