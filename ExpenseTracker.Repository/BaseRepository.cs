using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExpenseTracker.Repository.Interfaces;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

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


        public virtual async Task ExecuteStoredProcedure(string storedProcedureName, Dictionary<string, object> parameters)
        {
            using (SqlConnection connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public virtual async Task<List<D>> ExecuteStoredProcedure<D>(string storedProcedureName, Dictionary<string, object> parameters)
        {
            List<D> results = new List<D>();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    await connection.OpenAsync();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            D result = Activator.CreateInstance<D>();
                            foreach (PropertyInfo property in typeof(D).GetProperties())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                                {
                                    property.SetValue(result, reader[property.Name], null);
                                }
                            }
                            results.Add(result);
                        }
                    }
                }
            }
            return results;
        }

        public virtual async Task<DataSet> GetDataSet(string storedProcedureName, Dictionary<string, object> parameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = storedProcedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        await Task.Run(() => da.Fill(ds));
                    }
                }
            }
            return ds;
        }
    }
}