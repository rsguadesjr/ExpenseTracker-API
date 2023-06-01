using AutoMapper;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Mapping
{
    internal class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<Category, Option>();
            CreateMap<Category, CategoryResponseModel>();
            CreateMap<CategoryRequestModel, Category>();
        }
    }
}
