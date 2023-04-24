using ExpenseTracker.Model.Models.Summary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface ISummaryService
    {
        Task<List<TotalPerCategory>> GetTotalAmountPerCategory(DateTime startDate, DateTime endDate);
        Task<List<MonthlySummaryByYearResult>> GetSummaryByDateRange(DateTime startDate, DateTime endDate);
        Task<List<MonthlySummaryByYearResult>> GetMonthlySummaryByYear(int year);
        Task<List<DailyTotalAmount>> GetDailyTotalByDateRange(DateTime startDate, DateTime endDate);
    }
}
