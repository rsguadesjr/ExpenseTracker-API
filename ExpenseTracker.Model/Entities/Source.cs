using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(Source))]
    public class Source : IAuditable
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? UserId { get; set; }
        public int? Order { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
