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
        private readonly IUserRepository _userRepository;

        public SourceService(IRepository<Source> sourceRepository,
                                    IUserRepository userRepository)
        {
            _sourceRepository = sourceRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Option>> GetAll()
        {// Get current user
            var currentUser = _userRepository.GetCurrentUser();
            if (currentUser == null)
            {
                throw new ApplicationException("User not found");
            }

            var result = _sourceRepository.GetAll<Option>(x => x.UserId == currentUser.UserId);
            return await result.ToListAsync();
        }
    }
}
