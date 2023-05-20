using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(ReminderRepeat))]
    public class ReminderRepeat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ReminderId { get; set; }
        public virtual Reminder Reminder { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 1 - One Time, 2 - Daily, 3 - Weekly, 4 - Monthly, 5 - Yearly
        /// </summary>
        public int Type { get; set; }
        public bool? OnMonday { get; set; }
        public bool? OnTuesday { get; set; }
        public bool? OnWednesday { get; set; }
        public bool? OnThursday { get; set; }
        public bool? OnFriday { get; set; }
        public bool? OnSaturday { get; set; }
        public bool? OnSunday { get; set; }
    }
}
