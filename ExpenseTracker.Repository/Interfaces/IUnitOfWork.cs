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
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(DateTime? dateTime = null);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();


        public IRepository<Category> CategoryRepository { get; }
        public IRepository<Expense> ExpenseRepository { get; }
    }
}
