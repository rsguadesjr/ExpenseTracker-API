using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(Reminder))]
    public class Reminder : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpenseDate { get; set; }
        public int? Amount { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int? SourceId { get; set; }
        public virtual Source Source { get; set; }
        public string? Tags { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual ReminderRepeat ReminderRepeat { get; set; }
    }
}
