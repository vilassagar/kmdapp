using System;
using System.Data;
using System.Data.Common;

namespace SqlHelpers
{
   
    public abstract class ParameterFactory<T> where T : DbParameter
    {
        internal abstract T CreateParameter();
        public T CreateParameter(object theValue, string parameterName, ParameterDirection direction)
        {
            ValidateParameter(parameterName);

            object valueToUse = theValue;

            if (theValue == null)
            {
                valueToUse = DBNull.Value;
            }

            T parameter = CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = valueToUse;
            parameter.Direction = direction;

            return parameter;
        }

        private static void ValidateParameter(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException("cannot be null or empty");
            }
        }
    }
}
