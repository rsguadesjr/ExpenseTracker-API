using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseRequestModel> Get(Guid id);
        Task<PaginatedList<ExpenseResponseModel>> GetAll(BaseSearchParameter searchParam);
        Task<ExpenseResponseModel> Create(ExpenseRequestModel dto);
        Task<ExpenseResponseModel> Update(ExpenseRequestModel dto);
        Task Delete(Guid id);
    }
}
