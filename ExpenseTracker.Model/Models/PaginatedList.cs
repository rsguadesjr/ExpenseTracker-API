using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models
{
    public class PaginatedList<T>
    {
        public int? CurrentPage { get; set; }
        public int TotalRows { get; set; }
        public List<T> Data { get; set; }
    }
}
