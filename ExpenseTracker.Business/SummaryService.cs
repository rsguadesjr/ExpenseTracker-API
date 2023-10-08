using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Summary;
using ExpenseTracker.Model.Models.User;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    public class SummaryService : ISummaryService
    {
        private readonly IStoredProcedure _storedProcedure;
        private readonly CurrentUserDetails _currentUser;
        public SummaryService(IStoredProcedure storedProcedure,
                                IUserRepository userRepository)
        {
            _storedProcedure = storedProcedure;
            _currentUser = userRepository.GetCurrentUser();
        }

        public async Task<List<TotalPerCategory>> GetTotalAmountPerCategory(DateTime startDate, DateTime endDate)
        {
            var parameters = new List<StoredProcedureRequestParameter>
            {
                new StoredProcedureRequestParameter("StartDate", startDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("EndDate", endDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("UserId", _currentUser.UserId),
            };
            return await _storedProcedure.ExecuteStoredProcedure<TotalPerCategory>("GetTotalAmountPerCategoryByDateRange", parameters);
        }

        public async Task<List<TotalPerTag>> GetTotalAmountPerTag(DateTime startDate, DateTime endDate)
        {
            var parameters = new List<StoredProcedureRequestParameter>
            {
                new StoredProcedureRequestParameter("StartDate", startDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("EndDate", endDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("UserId", _currentUser.UserId),
            };
            return await _storedProcedure.ExecuteStoredProcedure<TotalPerTag>("GetTotalAmountPerTagByDateRange", parameters);
        }

        public async Task<List<MonthlySummaryByYearResult>> GetSummaryByDateRange(DateTime startDate, DateTime endDate)
        {
            var parameters = new List<StoredProcedureRequestParameter>
            {
                new StoredProcedureRequestParameter("StartDate", startDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("EndDate", endDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("UserId", _currentUser.UserId),
            };
            var result = await _storedProcedure.ExecuteStoredProcedure<MonthlySummaryByYearResult>("GetSummaryByDateRange", parameters);
            return result;
        }

        public async Task<List<MonthlySummaryByYearResult>> GetMonthlySummaryByYear(int year)
        {
            var parameters = new List<StoredProcedureRequestParameter>
            {
                new StoredProcedureRequestParameter("Year", year),
                new StoredProcedureRequestParameter("UserId", _currentUser.UserId),
            };
            var result = await _storedProcedure.ExecuteStoredProcedure<MonthlySummaryByYearResult>("GetMonthlySummaryByYear", parameters);
            return result;
        }

        public async Task<List<DailyTotalAmount>> GetDailyTotalByDateRange(DateTime startDate, DateTime endDate)
        {
            var parameters = new List<StoredProcedureRequestParameter>
            {
                new StoredProcedureRequestParameter("StartDate", startDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("EndDate", endDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("UserId", _currentUser.UserId),
            };
            return await _storedProcedure.ExecuteStoredProcedure<DailyTotalAmount>("GetDailyTotalByDateRange", parameters);
        }

        public async Task<List<TotalPerCategoryPerDate>> GetTotalAmountPerCategoryGroupByDate(DateTime startDate, DateTime endDate)
        {
            var parameters = new List<StoredProcedureRequestParameter>
            {
                new StoredProcedureRequestParameter("StartDate", startDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("EndDate", endDate, SqlDbType.DateTime2),
                new StoredProcedureRequestParameter("UserId", _currentUser.UserId),
            };
            return await _storedProcedure.ExecuteStoredProcedure<TotalPerCategoryPerDate>("GetTotalAmountPerCategoryGroupByDate", parameters);
        }
    }
}
