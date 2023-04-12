using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class UserVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
