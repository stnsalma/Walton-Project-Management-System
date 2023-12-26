using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ProjectManagement.Infrastructures.Helper
{
    public class OracleDbConnection
    {
        public static OracleConnection GetOldConnection()
        {
            const string connectionString = "Data Source=(DESCRIPTION="
                                            + "(ADDRESS=(PROTOCOL=TCP)(HOST=test)(PORT=1521))"
                                            + "(CONNECT_DATA=(SERVICE_NAME=test)));"
                                            + "User Id=test;Password=test;";

            var oldConnection = new OracleConnection(connectionString);
            return oldConnection;
        }

        public static OracleConnection GetNewConnection()
        {
            const string connectionString = "Data Source=(DESCRIPTION="
                                            + "(ADDRESS=(PROTOCOL=TCP)(HOST=test)(PORT=test))"
                                            + "(CONNECT_DATA=(SERVICE_NAME=PROD)));"
                                            + "User Id=test;Password=test#;";

            var newConnection = new OracleConnection(connectionString);
            return newConnection;
        }
    }
}
