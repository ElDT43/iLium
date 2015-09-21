using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using iLium.DataAccess;

namespace iLium
{
    public class MsSqlConnector : Connector
    {
        private MsSqlMethods _mssqlmethods;

        public MsSqlConnector(string connectionString)
        {
            _mssqlmethods = new MsSqlMethods(connectionString);
        }

        public MsSqlConnector(string connectionString, bool closeConnection)
        {
            _mssqlmethods = new MsSqlMethods(connectionString, closeConnection);
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

        public override void CloseConnection()
        { 
            _mssqlmethods.CloseConnection();
        }
    }
}
