using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
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
    }
}
