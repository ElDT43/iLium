using System.Collections.Generic;
using System.Data;

namespace iLium
{
    public class Initializer
    {
        public void Fill(System.Data.DataTable dataTable, string procedureName, System.Data.SqlClient.SqlParameter[] parameters, Connector cnn)
        {
            cnn.Fill(dataTable, procedureName, parameters);
        }

        public object ExecuteProcedure(string procedureName, System.Data.SqlClient.SqlParameter[] parameters, Connector cnn)
        {
            return cnn.ExecuteProcedure(procedureName, parameters);
        }

        public DataTable ExecuteQuery(string query, Connector cnn)
        {
            return cnn.ExecuteQuery(query);
        }

        public int ExecuteNonQuery(string query, System.Data.SqlClient.SqlParameter[] parameters, Connector cnn)
        {
            return cnn.ExecuteNonQuery(query, parameters);
        }

        public bool ExecuteBulkQuery(IList<string> _sqlQuery, Connector cnn) 
        {
            return cnn.ExecuteBulkQuery(_sqlQuery);
        }
    }
}
