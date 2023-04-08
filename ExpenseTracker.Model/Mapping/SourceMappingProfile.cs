using AutoMapper;
using ExpenseTracker.Model.Common;
using ExpenseTracker.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Mapping
{
    internal class SourceMappingProfile : Profile
    {
        public SourceMappingProfile()
        {
            CreateMap<Source, Option>();
        }
    }
}
