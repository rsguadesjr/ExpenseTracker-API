using AutoMapper;
using ExpenseTracker.Model.Mapping;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model
{
    public class DependencyInjection
    {
        public static void Initialize(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ExpenseMappingProfile());
                mc.AddProfile(new UserMappingProfile());
                mc.AddProfile(new CategoryMappingProfile());
                mc.AddProfile(new SourceMappingProfile());
                mc.AddProfile(new ReminderMappingProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
