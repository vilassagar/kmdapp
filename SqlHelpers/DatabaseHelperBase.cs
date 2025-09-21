using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
namespace SqlHelpers
{
    public abstract class DatabaseHelperBase : IDatabaseHelper
    {
        private static string _connectionString;
        public DatabaseHelperBase(string connectionString)
        {
            _connectionString = connectionString;
        }
        public abstract DbConnection CreateConnection();
        public abstract DbCommand CreateCommand();
        public abstract DbDataAdapter CreateDataAdapter(DbCommand command);

        #region Query Methods
        public virtual int ExecuteNonQuery<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            return ExecuteNonQueryAsync(commandType, commandText, commandParameters).TaskResult();
        }
        public virtual int ExecuteNonQuery<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters,out int outParameter) where TParameter : DbParameter
        {
            outParameter = 0;
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("Connection String cannot be null or empty");
            }

            using (DbConnection cn = CreateConnection())
            {
                cn.Open();
                using (DbCommand cmd = CreateCommand())
                {
                    PrepareCommand(cmd, cn, null, commandType, commandText, commandParameters);

                    // Finally, execute the command
                    int retval = cmd.ExecuteNonQuery();
                    if (retval <= 0) retval = 1;
                    var returnParameterName = commandParameters.Where(x => x.Direction == ParameterDirection.ReturnValue)?.FirstOrDefault()?.ParameterName;
                    if (!string.IsNullOrWhiteSpace(returnParameterName))
                        retval = Convert.ToInt32(cmd.Parameters[returnParameterName].Value);

                    var outParameterName = commandParameters.Where(x => x.Direction == ParameterDirection.Output)?.FirstOrDefault()?.ParameterName;
                    if (!string.IsNullOrWhiteSpace(outParameterName))
                        outParameter = Convert.ToInt32(cmd.Parameters[outParameterName].Value);

                    // Detach the DbParameters from the command object, so they can be used again
                    cmd.Parameters.Clear();
                    return retval;
                }
                // Call the overload that takes a connection in place of the connection string
            }
        }

        public virtual async Task<int> ExecuteNonQueryAsync<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("Connection String cannot be null or empty");
            }

            using (DbConnection cn = CreateConnection())
            {
                await cn.OpenAsync().ConfigureAwait(false);
             
                // Call the overload that takes a connection in place of the connection string
              return await ExecuteNonQueryAsync(cn, commandType, commandText, commandParameters).ConfigureAwait(false);
            }
        }

        public virtual async Task<int> ExecuteNonQueryAsync<TParameter>(DbConnection connection, CommandType commandType, string commandText
            , IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            // Create a command and prepare it for execution
            using (DbCommand cmd = CreateCommand())
            {
                await PrepareCommandAsync(cmd, connection, null, commandType, commandText, commandParameters).ConfigureAwait(false);

                // Finally, execute the command
                int retval = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                if (retval <= 0) retval = 1;
                var returnParameterName = commandParameters.Where(x => x.Direction == ParameterDirection.ReturnValue)?.FirstOrDefault()?.ParameterName;
                if (!string.IsNullOrWhiteSpace(returnParameterName))
                    retval = Convert.ToInt32(cmd.Parameters[returnParameterName].Value);

                // Detach the DbParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
                return retval;
            }
        }
       
        

        #endregion

        #region ExecuteScalar Methods
        public virtual SqlCommand ExecuteScalar<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            return ExecuteScalarAsync(commandType, commandText, commandParameters).TaskResult();
        }
        public virtual async Task<SqlCommand> ExecuteScalarAsync<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("cannot be null or empty");
            }

            using (DbConnection cn = CreateConnection())
            {
                await cn.OpenAsync().ConfigureAwait(false);

                // Call the overload that takes a connection in place of the connection string
                return await ExecuteScalarAsync(cn, commandType, commandText, commandParameters).ConfigureAwait(false);
            }
        }

        public virtual async Task<SqlCommand> ExecuteScalarAsync<TParameter>(DbConnection connection, CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            // Create a command and prepare it for execution
            using (DbCommand cmd = CreateCommand())
            {
                await PrepareCommandAsync(cmd, connection, null, commandType, commandText, commandParameters).ConfigureAwait(false);

                // Finally, execute the command
                object val = await cmd.ExecuteScalarAsync().ConfigureAwait(false);

                int.TryParse(Convert.ToString(val), out int retval);

                if (retval <= 0) retval = 1;
                var returnParameterName = commandParameters.Where(x => x.Direction == ParameterDirection.ReturnValue)?.FirstOrDefault()?.ParameterName;
                if (!string.IsNullOrWhiteSpace(returnParameterName))
                    retval = Convert.ToInt32(cmd.Parameters[returnParameterName].Value);

                // Detach the DbParameters from the command object, so they can be used again
                //cmd.Parameters.Clear();
                return (SqlCommand)cmd;
            }
        }

        #endregion

        #region ExecuteDataSet
        public virtual DataSet ExecuteDataSet<TParameter>(CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new ArgumentException("cannot be null or empty");
            }

            // Create & open a DbConnection, and dispose of it after we are done

            using (DbConnection cn = CreateConnection())
            {
                cn.Open();

                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataSet(cn, commandType, commandText, commandParameters);
            }
        }

        public virtual DataSet ExecuteDataSet<TParameter>(DbConnection connection, CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            // Create a command and prepare it for execution
            using (DbCommand cmd = CreateCommand())
            {
                PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters);

                // Create the DataAdapter & DataSet
                using (DbDataAdapter da = CreateDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    ds.Locale = CultureInfo.CurrentCulture;

                    // Fill the DataSet using default values for DataTable names, etc
                    da.Fill(ds);

                    // Detach the DbParameters from the command object, so they can be used again
                    cmd.Parameters.Clear();

                    // Geef de dataset terug.
                    return ds;
                }
            }
        }


        #endregion



        #region private methods
        //private static void AttachParameters<TParameter>(DbCommand command, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        //{
        //    foreach (TParameter dbParameter in commandParameters)
        //    {
        //        // If dbparameter doens't have a value, set it to DBNull.
        //        if ((dbParameter.Direction == ParameterDirection.Input || dbParameter.Direction == ParameterDirection.InputOutput) && (dbParameter.Value == null))
        //        {
        //            dbParameter.Value = DBNull.Value;
        //        }

        //        command.Parameters.Add(dbParameter);
        //    }
        //}
        private static void AttachParameters<TParameter>(DbCommand command, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            foreach (TParameter dbParameter in commandParameters)
            {
                // Handle GUID conversion for SqlParameter
                if (dbParameter is SqlParameter sqlParam)
                {
                    // If it's a UniqueIdentifier parameter but value is a string, convert it
                    if (sqlParam.SqlDbType == SqlDbType.UniqueIdentifier &&
                        sqlParam.Value != null &&
                        sqlParam.Value != DBNull.Value &&
                        sqlParam.Value is string stringValue)
                    {
                        if (Guid.TryParse(stringValue, out Guid guidValue))
                        {
                            sqlParam.Value = guidValue;
                        }
                        else if (string.IsNullOrEmpty(stringValue))
                        {
                            sqlParam.Value = DBNull.Value;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid GUID format for parameter '{sqlParam.ParameterName}': {stringValue}");
                        }
                    }

                    // Auto-detect GUID from string and set proper SqlDbType if not set
                    else if (sqlParam.SqlDbType == SqlDbType.Variant &&
                             sqlParam.Value is string guidString &&
                             Guid.TryParse(guidString, out Guid autoGuid))
                    {
                        sqlParam.SqlDbType = SqlDbType.UniqueIdentifier;
                        sqlParam.Value = autoGuid;
                    }
                }

                // If dbparameter doesn't have a value, set it to DBNull.
                if ((dbParameter.Direction == ParameterDirection.Input || dbParameter.Direction == ParameterDirection.InputOutput) &&
                    (dbParameter.Value == null))
                {
                    dbParameter.Value = DBNull.Value;
                }

                command.Parameters.Add(dbParameter);
            }
        }
        internal async Task PrepareCommandAsync<TParameter>(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, IEnumerable<TParameter> commandParameters) where TParameter : DbParameter
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection", "cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("cannot be null or empty");
            }

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync().ConfigureAwait(false);
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null && commandParameters.Count() > 0)
            {
                AttachParameters(command, commandParameters);
            }
        }
        internal void PrepareCommand(DbCommand command, DbConnection connection, DbTransaction transaction, CommandType commandType, string commandText, IEnumerable<DbParameter> commandParameters)
        {
#pragma warning disable 4014
            PrepareCommandAsync(command, connection, transaction, commandType, commandText, commandParameters).ConfigureAwait(false);
#pragma warning restore 4014
        }
        #endregion
    }
}
