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
    public class ReminderRepeatRepository : Repository<ReminderRepeat>
    {
        public ReminderRepeatRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<ReminderRepeat> Update(dynamic key, ReminderRepeat entity, List<string> properties, bool updateProperties = true)
        {
            var reminderId = (int)key;
            var entryInDb = await _context.ReminderRepeats.SingleAsync(x => x.ReminderId == reminderId);
            var ignoreProps = new List<string>
            {
                nameof(ReminderRepeat.Id),
                nameof(ReminderRepeat.Reminder),
            };
            _context.MapValueToDB(entity, entryInDb, ignoreProps, false);

            return entity;
        }
    }
}
