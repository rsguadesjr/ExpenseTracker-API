using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(Category))]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
