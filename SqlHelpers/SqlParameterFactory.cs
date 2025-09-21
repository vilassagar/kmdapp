using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SqlHelpers
{
    /// <summary>
    /// Factory for creating Sql Parameters.
    /// </summary>
    public class SqlParameterFactory : ParameterFactory<SqlParameter>
    {
        /// <summary>
        /// Creates the parameter.
        /// </summary>
        /// <returns></returns>
        internal override SqlParameter CreateParameter()
        {
            return new SqlParameter();
        }
    }
}
