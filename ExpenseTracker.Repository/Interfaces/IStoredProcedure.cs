using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IStoredProcedure
    {
        Task ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters);
        Task<List<D>> ExecuteStoredProcedure<D>(string storedProcedureName, Dictionary<string, object> parameters);
        Task<DataSet> GetDataSet(string storedProcedureName, Dictionary<string, object> parameters);
    }
}
