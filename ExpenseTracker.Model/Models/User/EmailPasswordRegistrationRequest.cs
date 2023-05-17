using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class EmailPasswordRegistrationRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        [Required]
        public string DisplayName { get; set; }

    }
}
