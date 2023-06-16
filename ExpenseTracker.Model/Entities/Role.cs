using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(Role))]
    public class Role : IAuditable
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; } = true;
        public bool IsActive { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedById { get; set; }
    }
}
