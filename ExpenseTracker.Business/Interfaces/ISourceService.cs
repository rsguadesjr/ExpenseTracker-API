using ExpenseTracker.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface ISourceService
    {
        Task<List<Option>> GetAll();
    }
}
