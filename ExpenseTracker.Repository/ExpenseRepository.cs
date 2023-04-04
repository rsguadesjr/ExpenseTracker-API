using AutoMapper;
using ExpenseTracker.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class ExpenseRepository : BaseRepository<Expense>
    {
        public ExpenseRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        //public override async Task<D> Get<D>(Expression<Func<Expense, bool>> predicate)
        //{
        //    var test = await _context.Expenses.FirstOrDefaultAsync(predicate);

        //    return default;
        //}

        public override async Task<Expense> Update(Expense entity)
        {
            var expense = await _context.Expenses.FirstAsync(x => x.Id == entity.Id);

            expense.Amount = entity.Amount;
            expense.ExpenseDate = entity.ExpenseDate;
            expense.Description = entity.Description;
            expense.CategoryId = entity.CategoryId;
            expense.SourceId = entity.SourceId;

            return expense;
        }
    }
}
