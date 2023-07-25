using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Model.Common
{
    public class StoredProcedureRequestParameter
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public SqlDbType? SqlDbType { get; set; }

        public StoredProcedureRequestParameter(string Key, object Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        public StoredProcedureRequestParameter(string Key, object Value, SqlDbType sqlDbType)
        {
            this.Key = Key;
            this.Value = Value;
            this.SqlDbType = sqlDbType;
        }

    }
}
