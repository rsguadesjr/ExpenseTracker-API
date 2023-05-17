using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Summary;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    // TODO: Correct the use of UserId
    public class SummaryService : ISummaryService
    {
        private readonly IRepository<Expense> _expenseRepository;
        private readonly CurrentUserDetails _currentUser;
        public SummaryService(IRepository<Expense> expenseRepository,
                                IUserRepository userRepository)
        {
            _expenseRepository = expenseRepository;
            _currentUser = userRepository.GetCurrentUser();
        }

        public async Task<List<TotalPerCategory>> GetTotalAmountPerCategory(DateTime startDate, DateTime endDate)
        {
            var parameters = new Dictionary<string, object>
            {
                { "StartDate", startDate },
                { "EndDate", endDate },
                { "UserId", _currentUser.UserId }

            };
            return await _expenseRepository.ExecuteStoredProcedure<TotalPerCategory>("GetTotalAmountPerCategoryByDateRange", parameters);
        }

        public async Task<List<TotalPerTag>> GetTotalAmountPerTag(DateTime startDate, DateTime endDate)
        {
            var parameters = new Dictionary<string, object>
            {
                { "StartDate", startDate },
                { "EndDate", endDate },
                { "UserId", _currentUser.UserId }

            };
            return await _expenseRepository.ExecuteStoredProcedure<TotalPerTag>("GetTotalAmountPerTagByDateRange", parameters);
        }

        public async Task<List<MonthlySummaryByYearResult>> GetSummaryByDateRange(DateTime startDate, DateTime endDate)
        {
            var parameters = new Dictionary<string, object>
            {
                { "StartDate", startDate },
                { "EndDate", endDate },
                { "UserId", _currentUser.UserId }

            };
            var result = await _expenseRepository.ExecuteStoredProcedure<MonthlySummaryByYearResult>("GetSummaryByDateRange", parameters);
            return result;
        }

        public async Task<List<MonthlySummaryByYearResult>> GetMonthlySummaryByYear(int year)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Year", year },
                { "UserId", Guid.Parse("1bd17662-e014-43d4-b380-097acd2c2ae6")}
            };
            var result = await _expenseRepository.ExecuteStoredProcedure<MonthlySummaryByYearResult>("GetMonthlySummaryByYear", parameters);
            return result;
        }

        public async Task<List<DailyTotalAmount>> GetDailyTotalByDateRange(DateTime startDate, DateTime endDate)
        {
            var parameters = new Dictionary<string, object>
            {
                { "StartDate", startDate },
                { "EndDate", endDate },
                { "UserId", _currentUser.UserId }

            };
            return await _expenseRepository.ExecuteStoredProcedure<DailyTotalAmount>("GetDailyTotalByDateRange", parameters);
        }
    }
}
