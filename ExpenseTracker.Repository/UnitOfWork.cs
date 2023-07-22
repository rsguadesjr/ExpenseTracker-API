using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ExpenseTrackerContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        public UnitOfWork(ExpenseTrackerContext context,
                          IHttpContextAccessor httpContext,
                          IUserRepository userRepository)
        {
            _context = context;
            _httpContext = httpContext;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime">Optional. This is use for logging the date for CreatedDate/ModifiedDate etc.</param>
        /// <returns></returns>
        public async Task SaveChangesAsync(DateTime? dateTime)
        {
            dateTime = dateTime ?? DateTime.UtcNow;
            var currentUser = _userRepository.GetCurrentUser();
            await _context.SaveChangesAsync(dateTime, currentUser?.UserId);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
