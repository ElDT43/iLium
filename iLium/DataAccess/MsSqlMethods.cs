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

        public MsSqlMethods(string connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
        }

        public void Fill(DataTable dataTable, string procedureName, params object[] parameters)
        {
            var cmd = new SqlCommand(procedureName, _sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            SqlDataAdapter sqlAdapter = new SqlDataAdapter();
            sqlAdapter.SelectCommand = cmd;

            _sqlConnection.Open();

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
                    if (_sqlConnection.State == ConnectionState.Open)
                        _sqlConnection.Close();

                    _sqlConnection.Dispose();

                    sqlAdapter.Dispose();

                    GC.SuppressFinalize(_sqlConnection);
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
                _sqlConnection.Open();

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
               if (_sqlConnection.State == ConnectionState.Open)
                    _sqlConnection.Close();

                _sqlConnection.Dispose();
                cmd.Dispose();

                GC.SuppressFinalize(_sqlConnection);
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
                _sqlConnection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                dt.Load(reader);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                    _sqlConnection.Close();

                _sqlConnection.Dispose();
                cmd.Dispose();
                GC.SuppressFinalize(_sqlConnection);
            }
            return dt;
        }

        public int ExecuteNonQuery(string query, params object[] parameters)
        {
            
            _sqlConnection.Open();

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
                    if (_sqlConnection.State == ConnectionState.Open)
                        _sqlConnection.Close();

                    _sqlConnection.Dispose();
                    GC.SuppressFinalize(_sqlConnection);
                }
            }

            return ret;
        }

        public bool ExecuteBulkQuery(IList<string> _sqlQuery)
        {
            if (_sqlQuery.Count() == 0) { return false; }

            _sqlConnection.Open();

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
                    //CONNECTION CLOSE AND DISPOSE
                    if (_sqlConnection.State == ConnectionState.Open) { _sqlConnection.Close(); }

                    _sqlConnection.Dispose();

                    GC.SuppressFinalize(_sqlConnection);
                }
            }

            return returnflag;
        }
    }
}
