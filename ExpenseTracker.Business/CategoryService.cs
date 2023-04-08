using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
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

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Option>> GetAll()
        {
            var result = _categoryRepository.GetAll<Option>(x => x.Id != 0);
            return await result.ToListAsync();
        }
    }
}
