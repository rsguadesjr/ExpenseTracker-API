using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Summary
{
    public class MonthlySummaryByYearResult
    {
        public int Total { get; set; }
        public Guid? UserId { get; set; }
        public string DisplayName { get; set; }
        public string ExpenseDate { get; set; }
    }
}
