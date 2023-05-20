using AutoMapper;
using ExpenseTracker.Business.Interfaces;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Reminder;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business
{
    public class ReminderService : IReminderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        public ReminderService(IUnitOfWork unitOfWork, IMapper mapper, IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<List<ReminderResponseModel>> GetAll(DateTime startDate, DateTime endDate)
        {
            var user = _userRepository.GetCurrentUser();
            if (user == null)
            {
                throw new ApplicationException("Invalid request!");
            }
            var result = await _unitOfWork.ReminderRepository.GetAll<ReminderResponseModel>(x => x.UserId == user.UserId ).ToListAsync();

            return result;

            //var reminderRepeat = _unitOfWork.ReminderRepeatRepository.GetAll(x => x.Reminder.UserId == user.UserId 
            //                                                                    && (!x.EndDate.HasValue || (startDate >= x.StartDate && x.StartDate <= endDate)));
            //var reminders = await _unitOfWork.ReminderRepository.GetAll(x => x.UserId == user.UserId
            //                                                                    && reminderRepeat.Select(y => y.ReminderId).Contains(x.Id)).ToListAsync();
            //var items = new List<ReminderDTO>();
            //foreach (var reminder in reminders)
            //{
            //    var item = _mapper.Map<ReminderDTO>(reminder);

            //    var repeat = reminderRepeat.FirstOrDefault(x => x.ReminderId == reminder.Id);
            //    if (repeat != null)
            //    {
            //        item.Repeat = _mapper.Map<ReminderRepeatDTO>(repeat);
            //    }

            //    items.Add(item);
            //}

            //return items;
        }

        public async Task<ReminderResponseModel> Get(int reminderId)
        {
            
            var reminder = await _unitOfWork.ReminderRepository.Get(x => x.Id == reminderId);
            if (reminder == null)
            {
                return null;
            }
            
            return await _unitOfWork.ReminderRepository.Get< ReminderResponseModel>(x => x.Id == reminder.Id);
            
        }

        public async Task<ReminderResponseModel> Create(ReminderRequestModel reminder)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var currentUser = _userRepository.GetCurrentUser();
                    var reminderEntity = _mapper.Map<Reminder>(reminder);
                    reminderEntity.UserId = currentUser.UserId;

                    var newReminder = (await _unitOfWork.ReminderRepository.Create(reminderEntity));
                    await _unitOfWork.SaveChangesAsync();
                    reminder.Id = newReminder.Id;

                    var repeatEntity = _mapper.Map<ReminderRepeat>(reminder);
                    await _unitOfWork.ReminderRepeatRepository.Create(repeatEntity);
                    await _unitOfWork.SaveChangesAsync();
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw ex;
                }
            }

            return await Get(reminder.Id.Value);
        }


        public async Task<ReminderResponseModel> Update(ReminderRequestModel reminder)
        {
            using (await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var reminderEntity = _mapper.Map<Reminder>(reminder);
                    await _unitOfWork.ReminderRepository.Update(reminderEntity);
                    await _unitOfWork.SaveChangesAsync();

                    var repeatEntity = _mapper.Map<ReminderRepeat>(reminder);
                    await _unitOfWork.ReminderRepeatRepository.Update(repeatEntity);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw ex;
                }
            }

            return await Get(reminder.Id.Value);
        }

        public async Task Delete(int id)
        {

            var repeatReminder = await _unitOfWork.ReminderRepeatRepository.Get(x => x.Reminder.Id == id);
            await _unitOfWork.ReminderRepository.Delete(id);
            await _unitOfWork.ReminderRepeatRepository.Delete(repeatReminder.Id);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
