using ExpenseTracker.Model.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserVM> Get(Guid id);
        Task<UserVM> GetByEmail(string email);
        Task<UserRegistrationResponse> Register();
        Task<AuthRequestResult> Register(EmailPasswordRegistrationRequest requestData);
        Task<AuthRequestResult> Login(string token);
    }
}
