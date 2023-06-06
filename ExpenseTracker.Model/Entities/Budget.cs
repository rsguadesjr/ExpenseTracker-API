using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(Budget))]
    public class Budget : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int Amount { get; set; }
        public short Month { get; set; }
        public short Year { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedById { get; set; }

        public virtual ICollection<BudgetCategory>? BudgetCategories { get; set; } = new List<BudgetCategory>();
    }
}
