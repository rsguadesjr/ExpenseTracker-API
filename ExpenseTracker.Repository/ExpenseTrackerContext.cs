using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models;
using ExpenseTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository
{
    public class ExpenseTrackerContext : DbContext
    {
        public ExpenseTrackerContext(DbContextOptions<ExpenseTrackerContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ExpenseTag> ExpenseTags { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<ReminderRepeat> ReminderRepeats { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExpenseTag>()
                .HasKey(a => new { a.ExpenseId, a.TagId });

            modelBuilder.Entity<Expense>()
                .HasMany(x => x.ExpenseTags)
                .WithOne(x => x.Expense)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public async Task<int> SaveChangesAsync(DateTime? dateTime, Guid? userId)
        {
            OnBerforeSaveChanges(dateTime, userId);
            return await base.SaveChangesAsync();

        }

        private async void OnBerforeSaveChanges(DateTime? dateTime, Guid? userId)
        {
            ChangeTracker.DetectChanges();

            var auditableEntries = ChangeTracker.Entries<IAuditable>().Where(x => x.Entity is IAuditable && x.State != EntityState.Unchanged).ToList();
            foreach (var entry in auditableEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = dateTime;
                        entry.Entity.CreatedById = userId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = dateTime;
                        entry.Entity.ModifiedById = userId;
                        break;
                }
            }
        }

        public void MapValueToDB<T>(T updatedEntry, T entryInDb, List<string> properties, bool updateProperties)
        {
            PropertyInfo[] props = typeof(T).GetProperties();

            foreach (PropertyInfo property in props)
            {
                if ((updateProperties && properties.Contains(property.Name))
                    || (!updateProperties && !properties.Contains(property.Name)))
                {
                    var val = property.GetValue(updatedEntry);
                    property.SetValue(entryInDb, val);
                }
            }
        }

    }
}
