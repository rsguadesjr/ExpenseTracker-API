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
    public class CategoryRepository : BaseRepository<Category>
    {
        public CategoryRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<Category> Update(Category entity)
        {
            var category = await _context.Categories.SingleAsync(x => x.Id == entity.Id);
            category.Name = entity.Name;
            category.Description = entity.Description;
            category.IsActive = entity.IsActive;
            
            return entity;
        }

    }
}
