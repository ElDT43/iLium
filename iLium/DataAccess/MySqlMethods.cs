using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace iLium.DataAccess
{
    public class MySqlMethods
    {
        private MySqlConnection _mySqlConnection;
        private bool _closeConnection;

        public MySqlMethods(string connectionString)
        {
            _mySqlConnection = new MySqlConnection(connectionString);
        }

        public MySqlMethods(string connectionString, bool closeConnection = true)
        {
            _mySqlConnection = new MySqlConnection(connectionString);
        }

        public void Fill(System.Data.DataTable dataTable, string procedureName, params object[] parameters)
        {
            var cmd = new MySqlCommand(procedureName, _mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            var mysqlAdapter = new MySqlDataAdapter();
            mysqlAdapter.SelectCommand = cmd;

            this.OpenConnection();

            using (MySqlTransaction mysqlTransaction = _mySqlConnection.BeginTransaction())
            {
                try
                {
                    mysqlAdapter.SelectCommand.Transaction = mysqlTransaction;
                    mysqlAdapter.Fill(dataTable);

                    mysqlTransaction.Commit();
                }
                catch
                {
                    mysqlTransaction.Rollback();

                    throw;
                }
                finally
                {
                    this.CloseConnection();

                    mysqlAdapter.Dispose();
                }
            }
        }

        public DataTable ExecuteProcedure(string procedureName, params object[] parameters)
        {
            var cmd = new MySqlCommand(procedureName, _mySqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            DataTable dt = new DataTable();
            try
            {
                this.OpenConnection();

                MySqlDataReader reader = cmd.ExecuteReader();

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
            MySqlCommand cmd = new MySqlCommand(query, _mySqlConnection);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 500;

            DataTable dt = new DataTable();

            try
            {
                this.OpenConnection();

                MySqlDataReader reader = cmd.ExecuteReader();

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

            using (MySqlTransaction transaction = _mySqlConnection.BeginTransaction())
            {
                try
                {
                    var cmd = new MySqlCommand(query, _mySqlConnection);
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

            var _returnflag = true;

            using (MySqlTransaction _transaction = _mySqlConnection.BeginTransaction())
            {
                try
                {
                    foreach (var item in _sqlQuery)
                    {
                        var cmd = new MySqlCommand(item, _mySqlConnection);

                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = _transaction;

                        cmd.ExecuteNonQuery();

                        cmd.Dispose();
                    }

                    _transaction.Commit();
                }
                catch
                {

                    _transaction.Rollback();

                    _returnflag = false;
                    throw;
                }
                finally
                {
                    this.CloseConnection();
                }
            }

            return _returnflag;
        }


        public void CloseConnection()
        {

            if (_closeConnection)
            {
                if (_mySqlConnection.State == ConnectionState.Open)
                {
                    _mySqlConnection.Close();

                    GC.SuppressFinalize(_mySqlConnection);
                    _mySqlConnection.Dispose();
                }
            }
        }

        private void OpenConnection()
        {
            if (_mySqlConnection.State != ConnectionState.Open) 
                _mySqlConnection.Open();
        }
    }
}
