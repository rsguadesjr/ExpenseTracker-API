using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Common
{
    public class RepositoryResponseModel<T>
    {
        public T Result { get; set; }
        public int? TotalRows { get; set; }
    }
}
