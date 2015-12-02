using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace iLium.DataAccess
{
    public class MsSqlMethods
    {
        private SqlConnection _sqlConnection;
        private bool _closeConnection;

        public MsSqlMethods(string connectionString, bool closeConnection = true)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _closeConnection = closeConnection;
        }

        public MsSqlMethods(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _closeConnection = true;
        }


        public void Fill(DataTable dataTable, string procedureName, params object[] parameters)
        {
            var cmd = new SqlCommand(procedureName, _sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = cmd;

            this.OpenConnection();

            using (SqlTransaction sqlTransaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    sqlAdapter.SelectCommand.Transaction = sqlTransaction;
                    sqlAdapter.Fill(dataTable);

                    sqlTransaction.Commit();
                }
                catch
                {
                    sqlTransaction.Rollback();

                    throw;
                }
                finally
                {
                    this.CloseConnection();

                    cmd.Dispose();
                }
            }
        }

        public DataTable ExecuteProcedure(string procedureName, params object[] parameters)
        {            
            var cmd = new SqlCommand(procedureName, _sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            DataTable dt = new DataTable();
            try
            {
                this.OpenConnection();

                SqlDataReader reader = cmd.ExecuteReader();

                dt.Load(reader);
                reader.Close();
            }
            catch 
            {
                throw;
            }
            finally
            {
                this.CloseConnection();

                cmd.Dispose();
            }

            return dt;
        }

        public DataTable ExecuteQuery(string query)
        {
            SqlCommand cmd = new SqlCommand(query, _sqlConnection);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 500;

            DataTable dt = new DataTable();

            try
            {
                this.OpenConnection();

                SqlDataReader reader = cmd.ExecuteReader();

                dt.Load(reader);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.CloseConnection();

                cmd.Dispose();
            }
            return dt;
        }

        public int ExecuteNonQuery(string query, params object[] parameters)
        {

            this.OpenConnection();

            int ret = 0;

            using (SqlTransaction transaction = _sqlConnection.BeginTransaction())
            {    
                try
                {
                    SqlCommand cmd = new SqlCommand(query, _sqlConnection);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 500;
                    cmd.Transaction = transaction;

                    ret = cmd.ExecuteNonQuery();

                    cmd.Dispose();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    this.CloseConnection();
                }
            }

            return ret;
        }

        public bool ExecuteBulkQuery(IList<string> _sqlQuery)
        {
            if (_sqlQuery.Count() == 0) { return false; }

            this.OpenConnection();

            var returnflag = true;

            using (SqlTransaction transaction = _sqlConnection.BeginTransaction())
            {
                try
                {
                    foreach (var item in _sqlQuery)
                    {

                        var cmd = new SqlCommand(item, _sqlConnection);

                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = transaction;

                        cmd.ExecuteNonQuery();

                        cmd.Dispose();
                    }

                    transaction.Commit();
                }
                catch
                {

                    transaction.Rollback();

                    returnflag = false;
                    throw;
                }
                finally
                {
                    this.CloseConnection();
                }
            }

            return returnflag;
        }

        private void CloseConnection()
        {
            if (_closeConnection)
            {

                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();

                    GC.SuppressFinalize(_sqlConnection);
                    _sqlConnection.Dispose();
                }
            }
        }

        public void KillConnections() 
        {
            if (_sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection.Close();

                GC.SuppressFinalize(_sqlConnection);
                _sqlConnection.Dispose();
            }
        }

        private void OpenConnection()
        {
            if (_sqlConnection.State != ConnectionState.Open)
                _sqlConnection.Open();
        }
    }
}
