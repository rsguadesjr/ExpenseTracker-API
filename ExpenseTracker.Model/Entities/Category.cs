using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(Category))]
    public class Category : IAuditable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public Guid? UserId { get; set; }
        public int? Order { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
