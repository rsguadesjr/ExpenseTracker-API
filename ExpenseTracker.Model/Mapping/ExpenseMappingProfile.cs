using AutoMapper;
using ExpenseTracker.Model.Common;
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
            //CreateMap<Expense, ExpenseRequestModel>()
            //    .ForMember(x => x.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            //    .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source != null ? src.Source.Name : string.Empty))
            //    .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.ExpenseTags.Select(et => et.Tag.Name)));

            CreateMap<ExpenseRequestModel, Expense>();
                //.ForMember(x => x.Category, opt => opt.Ignore())
                //.ForMember(x => x.Source, opt => opt.Ignore())
                //.ForMember(x => x.ExpenseTags, opt => opt.Ignore());

            CreateMap<Expense, ExpenseResponseModel>()
                .ForMember(x => x.Category, opt => opt.MapFrom(src => src.Category != null ? new Option { Id = src.CategoryId, Name = src.Category.Name } : null ))
                .ForMember(x => x.Source, opt => opt.MapFrom(src => src.Source != null ? new Option { Id = src.SourceId, Name = src.Source.Name } : null ))
                .ForMember(x => x.User, opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : string.Empty))
                .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.ExpenseTags.Select(et => et.Tag.Name)));
        }
    }
}
