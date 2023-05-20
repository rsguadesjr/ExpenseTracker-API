using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Reminder
{
    public class ReminderDTO
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpenseDate { get; set; }
        public int? Amount { get; set; }
        public int? CategoryId { get; set; }
        public int? SourceId { get; set; }
        public string Tags { get; set; }
        public virtual ReminderRepeatDTO Repeat { get; set; }
    }
}
