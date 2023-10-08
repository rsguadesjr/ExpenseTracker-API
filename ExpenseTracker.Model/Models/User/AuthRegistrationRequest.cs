using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class AuthRegistrationRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
