using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Mapping
{
    public class ExpenseMappingProfile : Profile
    {
        public ExpenseMappingProfile()
        {
            CreateMap<Expense, ExpenseDto>()
                .ForMember(x => x.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source != null ? src.Source.Name : string.Empty));
            CreateMap<ExpenseDto, Expense>()
                .ForMember(x => x.Category, opt => opt.Ignore())
                .ForMember(x => x.Source, opt => opt.Ignore());

            CreateMap<Expense, ExpenseListResult>()
                .ForMember(x => x.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source != null ? src.Source.Name : string.Empty))
                .ForMember(x => x.User, opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : string.Empty));
        }
    }
}
