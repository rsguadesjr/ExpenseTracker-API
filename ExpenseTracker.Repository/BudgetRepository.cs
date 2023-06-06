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
    public class BudgetRepository : BaseRepository<Budget>
    {
        public BudgetRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<Budget> Update(Budget entity)
        {
            var budget = await _context.Budgets.SingleAsync(x => x.Id == entity.Id);
            budget.Amount = entity.Amount;
            budget.Month = entity.Month;
            budget.Year = entity.Year;
            budget.IsActive = entity.IsActive;
            budget.IsDefault = entity.IsDefault;

            return entity;
        }
    }
}
