using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
    }
}
