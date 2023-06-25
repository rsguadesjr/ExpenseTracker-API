using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Budget
{
    public class BudgetRequestModel
    {
        public int? Id { get; set; }
        public int Amount { get; set; }
        public short Month { get; set; }
        public int Year { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public List<BudgetCategoryRequestModel> BudgetCategories { get; set; } = new List<BudgetCategoryRequestModel>();
    }
}
