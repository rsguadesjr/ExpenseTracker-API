using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class AuthRequestResult
    {
        public bool IsAuthorized { get; set; } = false;
        public bool IsNewUser { get; set; } = true;
        public bool IsProfileComplete { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public string Token { get; set; } = string.Empty;

    }
}
