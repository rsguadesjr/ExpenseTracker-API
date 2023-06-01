using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponseModel> Get(int id);
        Task<List<CategoryResponseModel>> GetAll();
        Task<CategoryResponseModel> Create(CategoryRequestModel data);
        Task<CategoryResponseModel> Update(CategoryRequestModel data);
        Task Delete(int id);
        Task Sort();
    }
}
