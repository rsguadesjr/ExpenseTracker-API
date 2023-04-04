using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models
{
    public class CurrentUserDetails
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string UniqueId { get; set; }
        public string DisplayName { get; set; }
    }
}
