using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Summary
{
    public class TotalPerCategory
    {
        public int Total { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
