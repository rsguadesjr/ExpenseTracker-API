using ExpenseTracker.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class StoredProcedure : IStoredProcedure
    {
        protected readonly ExpenseTrackerContext _context;
        public StoredProcedure(ExpenseTrackerContext context)
        {
            _context = context;
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
