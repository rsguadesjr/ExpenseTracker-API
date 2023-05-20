using AutoMapper;
using ExpenseTracker.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class ReminderRepeatRepository : BaseRepository<ReminderRepeat>
    {
        public ReminderRepeatRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<ReminderRepeat> Update(ReminderRepeat entity)
        {
            var reminder = await _context.ReminderRepeats.SingleAsync(x => x.ReminderId == entity.ReminderId);
            reminder.StartDate = entity.StartDate;
            reminder.EndDate = entity.EndDate;
            reminder.Type = entity.Type;
            reminder.OnMonday = entity.OnMonday;
            reminder.OnTuesday = entity.OnTuesday;
            reminder.OnWednesday = entity.OnWednesday;
            reminder.OnThursday = entity.OnThursday;
            reminder.OnFriday = entity.OnFriday;
            reminder.OnSaturday = entity.OnSaturday;
            reminder.OnSunday = entity.OnSunday;

            return entity;
        }
    }
}
