using AutoMapper;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Mapping
{
    internal class BudgetMappingProfile : Profile
    {
        public BudgetMappingProfile()
        {
            CreateMap<Budget, BudgetResponseModel>()
                .ForMember(x => x.BudgetCategories, opt => opt.MapFrom(src => src.BudgetCategories.Select(s => new BudgetCategoryResponseModel
                {
                    Amount = s.Amount,
                    BudgetId = s.BudgetId.Value,
                    CategoryId = s.CategoryId,
                    Category = s.Category != null ? s.Category.Name : string.Empty,
                })));
            CreateMap<BudgetRequestModel, Budget>();

            CreateMap<BudgetCategory, BudgetCategoryResponseModel>()
                .ForMember(x => x.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
            CreateMap<BudgetCategoryRequestModel, BudgetCategory>();
        }
    }
}
