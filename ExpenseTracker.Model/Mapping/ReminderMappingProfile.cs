using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Reminder;

namespace ExpenseTracker.Model.Mapping
{
    public class ReminderMappingProfile : Profile
    {
        public ReminderMappingProfile()
        {
            CreateMap<Reminder, ReminderDTO>()
                .ForMember(x => x.Repeat, opt => opt.Ignore());
            CreateMap<ReminderDTO, Reminder>();

            CreateMap<Reminder, ReminderResponseModel>()
                .ForMember(x => x.Type, opt => opt.MapFrom(src => src.ReminderRepeat.Type))
                .ForMember(x => x.StartDate, opt => opt.MapFrom(src => src.ReminderRepeat.StartDate))
                .ForMember(x => x.EndDate, opt => opt.MapFrom(src => src.ReminderRepeat.EndDate));

            CreateMap<ReminderRequestModel, Reminder>();
            CreateMap<ReminderRequestModel, ReminderRepeat>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ReminderId, opt => opt.MapFrom(src => src.Id.Value));

            CreateMap<ReminderRepeat, ReminderRepeatDTO>();
            CreateMap<ReminderRepeatDTO, ReminderRepeat>()
                .ForMember(x => x.Reminder, opt => opt.Ignore());
        }
    }
}
