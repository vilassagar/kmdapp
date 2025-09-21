using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace SqlHelpers
{
    public class MsSqlHelper : DatabaseHelperBase, IMsSqlHelper
    {
        private static string _connectionString;
        public MsSqlHelper(string connectionString):base(connectionString)
        {
            _connectionString = connectionString;
        }

        public override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }
        public override DbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        public override DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            return new SqlDataAdapter(command as SqlCommand);
        }
    }
}
