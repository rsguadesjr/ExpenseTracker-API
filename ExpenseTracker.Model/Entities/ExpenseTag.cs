using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(ExpenseTag))]
    public class ExpenseTag
    {
        [Key]
        public Guid ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }
        [Key]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
