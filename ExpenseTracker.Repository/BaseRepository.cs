using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Repository.Interfaces;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repository
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ExpenseTrackerContext _context;
        private readonly IMapper _mapper;
        public BaseRepository(ExpenseTrackerContext context,
                                IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public virtual async Task<T> Create(T entity)
        {
             return (await _context.AddAsync(entity)).Entity;
        }

        public abstract Task<T> Update(T entity);
        public virtual Task<T> Delete(dynamic id)
        {
            throw new NotImplementedException();
        }

        public virtual IQueryable<D> GetAll<D>(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>()
                .Where(predicate)
                .ProjectTo<D>(_mapper.ConfigurationProvider)
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

        public virtual async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}