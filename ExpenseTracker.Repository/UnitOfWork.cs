using AutoMapper;
using ExpenseTracker.Model.Entities;
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
        private ExpenseTrackerContext _context;
        private IRepository<Category> _categoryRepository;
        private IRepository<Expense> _expenseRepository;
        private IRepository<Tag> _tagRepository;
        private IRepository<ExpenseTag> _expenseTagRepository;
        public IRepository<Reminder> _reminderRepository { get; set; }
        public IRepository<ReminderRepeat> _reminderRepeatRepository { get; set; }
        private IUserRepository _userRepository;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContext;
        public UnitOfWork(ExpenseTrackerContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime">Optional. This is use for logging the date for CreatedDate/ModifiedDate etc.</param>
        /// <returns></returns>
        public async Task SaveChangesAsync(DateTime? dateTime)
        {
            dateTime = dateTime ?? DateTime.UtcNow;
            var user = UserRepository.GetCurrentUser();
            await _context.SaveChangesAsync(dateTime, user?.UserId);
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

        public IRepository<Category> CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_context, _mapper);
                }

                return _categoryRepository;
            }
        }

        public IRepository<Expense> ExpenseRepository
        {
            get
            {
                if (_expenseRepository == null)
                {
                    _expenseRepository = new ExpenseRepository(_context, _mapper);
                }

                return _expenseRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context, _mapper, _httpContext);
                }

                return _userRepository;
            }
        }

        public IRepository<Tag> TagRepository
        {
            get
            {
                if (_tagRepository == null)
                {
                    _tagRepository = new TagRepository(_context, _mapper);
                }

                return _tagRepository;
            }
        }

        public IRepository<ExpenseTag> ExpenseTagRepository
        {
            get
            {
                if (_expenseTagRepository == null)
                {
                    _expenseTagRepository = new ExpenseTagRepository(_context, _mapper);
                }

                return _expenseTagRepository;
            }
        }

        public IRepository<Reminder> ReminderRepository
        {
            get
            {
                if (_reminderRepository == null)
                {
                    _reminderRepository = new ReminderRepository(_context, _mapper);
                }

                return _reminderRepository;
            }
        }

        public IRepository<ReminderRepeat> ReminderRepeatRepository
        {
            get
            {
                if (_reminderRepeatRepository == null)
                {
                    _reminderRepeatRepository = new ReminderRepeatRepository(_context, _mapper);
                }

                return _reminderRepeatRepository;
            }
        }
    }
}
