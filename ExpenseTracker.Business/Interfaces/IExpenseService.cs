using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseDto> Get(Guid id);
        Task<IEnumerable<ExpenseListResult>> GetAll(BaseSearchParameter searchParam);
        Task<ExpenseListResult> Create(ExpenseDto dto);
        Task<ExpenseListResult> Update(ExpenseDto dto);
        Task Delete(Guid id);
    }
}
