using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iLium.Test
{
    [TestClass]
    public class MySqlTest
    {
        [TestMethod]
        public void MySqlExecuteQueryTest()
        {
            var sqlhelper = new Initializer();

            Connector mySqlConnector = new MySqlConnector("Server=127.0.0.1;Database=dboptimove;Uid=astro;Pwd=1qazxc;Allow Zero Datetime=False;Convert Zero Datetime=True");
            var dt = (System.Data.DataTable)sqlhelper.ExecuteQuery("SELECT * FROM promotions;", mySqlConnector);

            Assert.IsNotNull(dt);
            //Assert.AreEqual(2, dt.Rows.Count);
        }
    }
}
