using ExpenseTracker.Model.Entities;
using ExpenseTracker.Model.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        CurrentUserDetails GetCurrentUser();
    }
}
