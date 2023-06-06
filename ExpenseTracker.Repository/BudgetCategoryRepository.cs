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
    public class BudgetCategoryRepository : BaseRepository<BudgetCategory>
    {
        public BudgetCategoryRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<BudgetCategory> Update(BudgetCategory entity)
        {
            var category = await _context.BudgetCategories.SingleAsync(x => x.Id == entity.Id);
            category.Amount = entity.Amount;
            category.CategoryId = entity.CategoryId;

            return entity;
        }
    }
}
