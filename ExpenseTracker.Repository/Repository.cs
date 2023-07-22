using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Repository.Interfaces;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExpenseTracker.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ExpenseTrackerContext _context;
        private readonly IMapper _mapper;
        public Repository(ExpenseTrackerContext context,
                                IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public virtual async Task<T> Create(T entity)
        {
             return (await _context.AddAsync(entity)).Entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Id of the entity</param>
        /// <param name="entity">Entity object</param>
        /// <param name="properties">Properties to update or ignore</param>
        /// <param name="updateProperties">If true, passed properties will be updated, else if false passed properties will be skipped</param>
        public virtual async Task Update(dynamic key, T entity, List<string> properties, bool updateProperties = true)
        {
            T dbRecord = await _context.Set<T>().FindAsync(key);
            _context.MapValueToDB(entity, dbRecord, properties, updateProperties);
        }
        public virtual async Task Delete(dynamic id)
        {
            T t = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(t);
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public virtual IQueryable<D> GetAll<D>(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>()
                .Where(predicate)
                .ProjectTo<D>(_mapper.ConfigurationProvider)
                .AsQueryable();
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>()
                .Where(predicate)
                .AsQueryable();
        }

        public virtual async Task<D> Get<D>(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .AsQueryable()
                .Where(predicate)
                .ProjectTo<D>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public virtual async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .AsQueryable()
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public virtual async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}