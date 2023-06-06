using ExpenseTracker.Model.Models.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface IBudgetService
    {
        Task<List<BudgetResponseModel>> GetAll();
        Task<BudgetResponseModel> Get(int id);
        Task<BudgetResponseModel> Create(BudgetRequestModel data);
        Task<BudgetResponseModel> Update(BudgetRequestModel data);
        Task Delete(int id);
    }
}
