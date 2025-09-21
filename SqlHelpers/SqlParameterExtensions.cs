using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SqlHelpers
{
    
    public static class SqlParameterExtensions
    {
        private static readonly SqlParameterFactory Factory = new SqlParameterFactory();

        private const ParameterDirection DefaultDirectionInputDirection = ParameterDirection.Input;

        public static SqlParameter Clone(this SqlParameter sqlParameter)
        {
            ICloneable cloneable = sqlParameter;
            return cloneable == null ? null : cloneable.Clone() as SqlParameter;
        }

        public static SqlParameter CreateSqlParameter<T>(this T? nullableObject, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection) where T : struct
        {
            return Factory.CreateParameter(nullableObject, parameterName, direction);
        }

        public static SqlParameter CreateSqlParameter(this string theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this DataTable theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this DateTime theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this int? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }

        public static SqlParameter CreateSqlParameter(this int theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateReturnValueSqlParameter(this int theValue, string parameterName, ParameterDirection direction = ParameterDirection.ReturnValue)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this decimal? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }

        public static SqlParameter CreateSqlParameter(this decimal theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this bool theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }

       
        public static SqlParameter CreateSqlParameter(this DateTime theValue, string parameterName, SqlDbType dateTimeType, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            SqlParameter sqlParameter = Factory.CreateParameter(theValue, parameterName, direction);
            sqlParameter.SqlDbType = dateTimeType;
            return sqlParameter;
        }

        public static SqlParameter CreateSqlParameter(this bool? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this DateTime? theValue, string parameterName, SqlDbType dateTimeType, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            if (theValue.HasValue)
            {
                CreateSqlParameter(theValue.Value, parameterName, dateTimeType, direction);
            }

            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this long? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this long theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this byte theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this byte? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this TimeSpan theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
        public static SqlParameter CreateSqlParameter(this TimeSpan? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }

        
        public static SqlParameter CreateSqlParameter(this float theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
       
        public static SqlParameter CreateSqlParameter(this float? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }

     
        public static SqlParameter CreateSqlParameter(this Guid theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }

       
        public static SqlParameter CreateSqlParameter(this Guid? theValue, string parameterName, ParameterDirection direction = DefaultDirectionInputDirection)
        {
            return Factory.CreateParameter(theValue, parameterName, direction);
        }
    }
}
