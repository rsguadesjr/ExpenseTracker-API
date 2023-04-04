using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IRepository<T>
    {
        Task<T> Create(T entity);
        abstract Task<T> Update(T entity);
        Task<T> Delete(dynamic id);
        Task<D> Get<D>(Expression<Func<T, bool>> predicate);
        IQueryable<D> GetAll<D>(Expression<Func<T, bool>> predicate);
        Task SaveChanges();
    }
}
