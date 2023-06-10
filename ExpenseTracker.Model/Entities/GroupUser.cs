using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Entities
{
    [Table(nameof(GroupUser))]
    public class GroupUser : IAuditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsAccepted { get; set; } = false;
        public DateTime? AcceptedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? ModifiedById { get; set; }
        public virtual User ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
