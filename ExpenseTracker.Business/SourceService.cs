using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    internal class SourceService : ISourceService
    {
        private readonly IRepository<Source> _sourceRepository;

        public SourceService(IRepository<Source> sourceRepository)
        {
            _sourceRepository = sourceRepository;
        }

        public async Task<List<Option>> GetAll()
        {
            var result = _sourceRepository.GetAll<Option>(x => x.Id != 0);
            return await result.ToListAsync();
        }
    }
}
