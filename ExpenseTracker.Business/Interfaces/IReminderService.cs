using ExpenseTracker.Model.Models.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface IReminderService
    {
        public Task<List<ReminderResponseModel>> GetAll(DateTime? startDate, DateTime? endDate);
        public Task<ReminderResponseModel> Get(int reminderId);
        public Task<ReminderResponseModel> Create(ReminderRequestModel dto);
        public Task<ReminderResponseModel> Update(ReminderRequestModel dto);
        public Task Delete(int id);
    }
}
