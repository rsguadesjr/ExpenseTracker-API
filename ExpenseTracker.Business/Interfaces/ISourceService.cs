using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface ISourceService
    {
        Task<List<SourceResponseModel>> GetAll();
    }
}
