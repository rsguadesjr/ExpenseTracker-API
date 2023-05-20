using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Reminder
{
    public class ReminderRequestModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = $"{nameof(Subject)} is required")]
        public string Subject { get; set; }
        public string? Note { get; set; }
        [Required(ErrorMessage = $"{nameof(StartDate)} is required")]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = $"{nameof(Type)} is required")]
        public int Type { get; set; }
        public Guid UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpenseDate { get; set; }
        public int? Amount { get; set; }
        public int? CategoryId { get; set; }
        public int? SourceId { get; set; }
        public string Tags { get; set; }
    }
}
