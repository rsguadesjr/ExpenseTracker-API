using AutoMapper;
using ExpenseTracker.Model.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Interfaces
{
    // TODO: re-evaluate implementation
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(DateTime? dateTime = null);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();


        public IRepository<Category> CategoryRepository { get; }
        public IRepository<Expense> ExpenseRepository { get; }
        public IRepository<Tag> TagRepository { get; }
        public IRepository<ExpenseTag> ExpenseTagRepository { get; }
        public IRepository<Reminder> ReminderRepository { get; }
        public IRepository<ReminderRepeat> ReminderRepeatRepository { get; }
        public IRepository<Budget> BudgetRepository { get; }
        public IRepository<BudgetCategory> BudgetCategoryRepository { get; }
        public IRepository<Group> GroupRepository { get; }
        public IRepository<GroupUser> GroupUserRepository { get; }
    }
}
