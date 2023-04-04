using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models
{
    public class UserRegistrationResponse
    {
        public bool IsSuccess { get; set; }
        public string EmailVerificationLink { get; set; }
        public List<string> ErrorMessages { get; set; }
        public UserRegistrationResponse()
        {
            ErrorMessages = new List<string>();
            IsSuccess = false;
        }
    }
}
