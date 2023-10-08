using ExpenseTracker.Model.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Business.Interfaces
{
    public interface IAccountService
    {
        Task<UserVM> Get(Guid id);
        Task<UserVM> GetByEmail(string email);
        Task SendUserGroupRequest(string email);
        Task AcceptUserGroupRequest(int groupId);
        Task<UserRegistrationResponse> Register(EmailPasswordRegistrationRequest requestData);
        Task<AuthRequestResult> Login(string token);
        //Task<UserRegistrationResponse> Register(string token);
    }
}
