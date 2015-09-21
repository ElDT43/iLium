using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using iLium.DataAccess;
using System.Data.SqlClient;

namespace iLium
{
    public class MySqlConnector : Connector
    {
        private MySqlMethods _mysqlmethods;

        public MySqlConnector(string connectionString)
        {
            _mysqlmethods = new MySqlMethods(connectionString);
        }

        public MySqlConnector(string connectionString, bool closeConnection)
        {
            _mysqlmethods = new MySqlMethods(connectionString, closeConnection);
        }

        public override void Fill(DataTable dataTable, string procedureName, params object[] parameters)
        {
            _mysqlmethods.Fill(dataTable, procedureName, parameters);
        }

        public override DataTable ExecuteProcedure(string procedureName, params object[] parameters)
        {
            return _mysqlmethods.ExecuteProcedure(procedureName, parameters);
        }

        public override DataTable ExecuteQuery(string query)
        {
            return _mysqlmethods.ExecuteQuery(query);
        }

        public override int ExecuteNonQuery(string query, params object[] parameters)
        {
            return _mysqlmethods.ExecuteNonQuery(query, parameters);
        }

        public override bool ExecuteBulkQuery(IList<string> _sqlQuery)
        {
            return _mysqlmethods.ExecuteBulkQuery(_sqlQuery);
        }
        
        public override void CloseConnection()
        {
            _mysqlmethods.CloseConnection();
        }
    }
}
