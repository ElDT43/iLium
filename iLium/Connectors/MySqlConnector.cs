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
        private MySqlMethods _mssqlmethods;

        public MySqlConnector(string connectionString)
        {
            _mssqlmethods = new MySqlMethods(connectionString);
        }

        public override void Fill(DataTable dataTable, string procedureName, params object[] parameters)
        {
            _mssqlmethods.Fill(dataTable, procedureName, parameters);
        }

        public override DataTable ExecuteProcedure(string procedureName, params object[] parameters)
        {
            return _mssqlmethods.ExecuteProcedure(procedureName, parameters);
        }

        public override DataTable ExecuteQuery(string query)
        {
            return _mssqlmethods.ExecuteQuery(query);
        }

        public override int ExecuteNonQuery(string query, params object[] parameters)
        {
            return _mssqlmethods.ExecuteNonQuery(query, parameters);
        }

        public override bool ExecuteBulkQuery(IList<string> _sqlQuery)
        {
            return _mssqlmethods.ExecuteBulkQuery(_sqlQuery);
        }
    }
}
