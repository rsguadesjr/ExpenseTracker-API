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
        Task Update(dynamic key, T entity, List<string> properties, bool updateProperties = true);
        Task Delete(dynamic id);
        void Delete(T entity);
        Task<D> Get<D>(Expression<Func<T, bool>> predicate);
        Task<T> Get(Expression<Func<T, bool>> predicate);
        IQueryable<D> GetAll<D>(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
        Task SaveChanges();
        Task ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters);
        Task<List<D>> ExecuteStoredProcedure<D>(string storedProcedureName, Dictionary<string, object> parameters);
        Task<DataSet> GetDataSet(string storedProcedureName, Dictionary<string, object> parameters);
    }
}
