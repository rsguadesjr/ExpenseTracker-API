using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Expense
{
    public class ExpenseListResult
    {
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? User { get; set; }
        public string Source { get; set; }
    }
}
