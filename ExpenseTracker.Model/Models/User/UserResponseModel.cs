using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class UserResponseModel
    {
        public Guid Id { get; set; }
        public string UniqueId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? SuffixName { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsProfileComplete { get; set; }
        public List<RoleResponseModel> Roles { get; set; }

    }
}
