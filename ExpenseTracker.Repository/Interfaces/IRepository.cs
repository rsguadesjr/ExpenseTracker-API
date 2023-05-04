using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> Create(T entity);
        abstract Task<T> Update(T entity);
        Task Delete(dynamic id);
        Task<D> Get<D>(Expression<Func<T, bool>> predicate);
        IQueryable<D> GetAll<D>(Expression<Func<T, bool>> predicate);
        Task SaveChanges();
        Task ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters);
        Task<List<D>> ExecuteStoredProcedure<D>(string storedProcedureName, Dictionary<string, object> parameters);
        Task<DataSet> GetDataSet(string storedProcedureName, Dictionary<string, object> parameters);
    }
}
