using ExpenseTracker.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Expense
{
    public class ExpenseResponseModel
    {
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public string? Description { get; set; }
        public Option Category { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? User { get; set; }
        public Option Source { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
