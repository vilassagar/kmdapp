using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelpers
{
    public interface IDatabaseHelper
    {  
        DbConnection CreateConnection();
        DbCommand CreateCommand();

        DbDataAdapter CreateDataAdapter(DbCommand command);

        #region    Query Methods
        int ExecuteNonQuery<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter;
        int ExecuteNonQuery<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters, out int outParameter) where TParameter : DbParameter;
        Task<int> ExecuteNonQueryAsync<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter;
        DataSet ExecuteDataSet<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter;
        SqlCommand ExecuteScalar<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter;

        #endregion


    }
}
