using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace iLium
{
    public abstract class Connector
    {
        public abstract void Fill(System.Data.DataTable dataTable, string procedureName, params object[] parameters);
        public abstract DataTable ExecuteProcedure(string procedureName, params object[] parameters);
        public abstract DataTable ExecuteQuery(string query);
        public abstract int ExecuteNonQuery(string query, params object[] parameters);
        public abstract bool ExecuteBulkQuery(IList<string> _sqlQuery);
        public abstract void KillConnections();
    }
 }
