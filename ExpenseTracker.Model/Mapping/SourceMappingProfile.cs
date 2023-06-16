using AutoMapper;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Models.Source;

namespace ExpenseTracker.Model.Mapping
{
    internal class SourceMappingProfile : Profile
    {
        public SourceMappingProfile()
        {
            CreateMap<ExpenseTracker.Model.Entities.Source, Option>();
            CreateMap<ExpenseTracker.Model.Entities.Source, SourceResponseModel>();
        }
    }
}
