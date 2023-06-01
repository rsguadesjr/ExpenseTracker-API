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
    public class ReminderRepository : BaseRepository<Reminder>
    {

        public ReminderRepository(ExpenseTrackerContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override async Task<Reminder> Update(Reminder entity)
        {
            var reminder = await _context.Reminders.SingleAsync(x => x.Id == entity.Id);
            reminder.Subject = entity.Subject;
            reminder.Note = entity.Note;
            reminder.Date = entity.Date;
            reminder.IsActive = entity.IsActive;
            reminder.ExpenseDate = entity.ExpenseDate;
            reminder.Amount = entity.Amount;
            reminder.CategoryId = entity.CategoryId;
            reminder.SourceId = entity.SourceId;
            reminder.Tags = entity.Tags;

            return entity;
        }
    }
}
