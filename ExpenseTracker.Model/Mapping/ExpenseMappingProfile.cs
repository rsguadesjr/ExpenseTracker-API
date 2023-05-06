using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Expense;
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
                .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source != null ? src.Source.Name : string.Empty))
                .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.ExpenseTags.Select(et => et.Tag.Name)));

            CreateMap<ExpenseDto, Expense>()
                .ForMember(x => x.Category, opt => opt.Ignore())
                .ForMember(x => x.Source, opt => opt.Ignore())
                .ForMember(x => x.ExpenseTags, opt => opt.Ignore());

            CreateMap<Expense, ExpenseListResult>()
                .ForMember(x => x.CategoryId, opt => opt.MapFrom(src => src.Category != null ? src.Category.Id : 0))
                .ForMember(x => x.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source != null ? src.Source.Name : string.Empty))
                .ForMember(x => x.User, opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : string.Empty))
                .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.ExpenseTags.Select(et => et.Tag.Name)));
        }
    }
}
