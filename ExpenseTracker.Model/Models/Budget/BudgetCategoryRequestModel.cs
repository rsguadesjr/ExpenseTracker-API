﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Models.Budget
{
    public class BudgetCategoryRequestModel
    {
        public int? BudgetId { get; set; }
        public string Amount { get; set; }
        public int CategoryId { get; set; }
    }
}