using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Collections.Generic;

namespace iLium.Test
{
    [TestClass]
    public class MsSqlTest
    {
        [TestMethod]
        public void MsExecuteQueryTest()
        {
            var sqlhelper = new Initializer();
            Connector databaseConnector = new MsSqlConnector(GetConnectionString("cnniLiumDatabaseTest"));

            var dt = (System.Data.DataTable)sqlhelper.ExecuteQuery("SELECT TOP 2 * FROM Users ", databaseConnector);

            Assert.IsNotNull(dt);
            Assert.AreEqual(2, dt.Rows.Count);
        }

        [TestMethod]
        public void MsExecuteProcedureTest()
        {
            var sqlhelper = new Initializer();
            Connector databaseConnector = new MsSqlConnector(GetConnectionString("cnniLiumDatabaseTest"));

            var parameters = new List<System.Data.SqlClient.SqlParameter>
            {    
                new System.Data.SqlClient.SqlParameter("@UserId", System.Data.SqlDbType.Int) { Value = 2 },
            };

            var dt = (System.Data.DataTable)sqlhelper.ExecuteProcedure("sp_GetUserById", parameters.ToArray(), databaseConnector);

            Assert.IsNotNull(dt);
            Assert.AreEqual(1, dt.Rows.Count);
        }

        public string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
