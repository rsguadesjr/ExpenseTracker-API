using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Budget
{
    public class BudgetResponseModel
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public short Month { get; set; }
        public short Year { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<BudgetCategoryResponseModel> BudgetCategories { get; set; } = new List<BudgetCategoryResponseModel>();
    }
}
