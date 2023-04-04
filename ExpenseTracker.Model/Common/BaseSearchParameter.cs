using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Common
{
    public class BaseSearchParameter
    {
        public Guid? UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? CategoryId { get; set; }
        public int? SourceId { get; set; }
        public int? PageNumber { get; set; }
        public int? TotalRows { get; set; }
    }
}
