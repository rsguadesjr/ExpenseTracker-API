using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Budget
{
    public class BudgetCategoryResponseModel
    {
        public int BudgetId { get; set; }
        public int Amount { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
