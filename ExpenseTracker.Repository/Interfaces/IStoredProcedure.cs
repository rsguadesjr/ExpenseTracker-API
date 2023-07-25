using ExpenseTracker.Model.Common;
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
        Task ExecuteStoredProcedure(string storedProcedureName, List<StoredProcedureRequestParameter> parameters);
        Task<List<D>> ExecuteStoredProcedure<D>(string storedProcedureName, List<StoredProcedureRequestParameter> parameters);
        Task<DataSet> GetDataSet(string storedProcedureName, List<StoredProcedureRequestParameter> parameters);
    }
}
