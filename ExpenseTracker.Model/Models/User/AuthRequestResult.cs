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
        public bool NeedToCompleteProfile { get; set; } = true;
        public string Token { get; set; } = string.Empty;

    }
}
