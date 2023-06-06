using ExpenseTracker.Model.Entities;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class DependencyInjection
    {
        public static void Initialize(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ExpenseTrackerContext>(opt => opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddScoped<IRepository<Expense>, ExpenseRepository>();
            services.AddScoped<IRepository<Category>, CategoryRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();
            services.AddScoped<IRepository<Source>, SourceRepository>();
            services.AddScoped<IRepository<Tag>, TagRepository>();
            services.AddScoped<IRepository<ExpenseTag>, ExpenseTagRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepository<Reminder>, ReminderRepository>();
            services.AddScoped<IRepository<ReminderRepeat>, ReminderRepeatRepository>();
            services.AddScoped<IRepository<Budget>, BudgetRepository>();
            services.AddScoped<IRepository<BudgetCategory>, BudgetCategoryRepository>();
        }
    }
}
