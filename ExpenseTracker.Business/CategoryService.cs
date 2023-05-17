using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUserRepository _userRepository;

        public CategoryService(IRepository<Category> categoryRepository,
                                IUserRepository userRepository)
        {
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Option>> GetAll()
        {  
            // Get current user
            var currentUser = _userRepository.GetCurrentUser();
            if (currentUser == null)
            {
                throw new ApplicationException("User not found");
            }

            var result = _categoryRepository.GetAll<Option>(x => x.UserId == currentUser.UserId);
            return await result.ToListAsync();
        }
    }
}
