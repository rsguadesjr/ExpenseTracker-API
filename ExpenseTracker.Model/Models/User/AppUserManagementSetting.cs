using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.User
{
    public class AppUserManagementSetting
    {
        public bool DisablePasswordReset { get; set; }
        public bool DisableRegistration { get; set; }
        public bool DisableEmailPasswordLogin { get; set; }
        public bool AllowSocialLogin { get; set; }
    }
}
