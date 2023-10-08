using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class UserRegistrationResponse
    {
        public bool AllowToLogin { get; set; } = false;
        public bool IsSuccess { get; set; } = false;
        public string EmailVerificationLink { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}
