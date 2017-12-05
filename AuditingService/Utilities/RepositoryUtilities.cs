using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Utilities
{
    public static class RepositoryUtilities
    {
        public static string CreateArrayContainsStatementForQuery(string propertyName, SqlParameterCollection parameters, params string[] values)
        {
            string arrayContainsPart = "";
            int parameterIndex = 0;
            int lastIndex = values.Length - 1;
            string propertyNameDotsReplacedWithUnderscores = propertyName.Replace(".", "_");
            foreach (string value in values)
            {
                string parameterName = $"@{propertyNameDotsReplacedWithUnderscores}_{parameterIndex}";
                arrayContainsPart += $"ARRAY_CONTAINS({propertyName}, {parameterName})";
                parameters.Add(new SqlParameter(parameterName, value));
                if (parameterIndex < lastIndex)
                {
                    arrayContainsPart += " OR ";
                }
                parameterIndex++;
            }
            return arrayContainsPart;
        }
        /// <summary>
        /// Creates an IN statement for a SQL query. For maintaining SQL property name uniqueness, the propertyname is used as part of the 
        /// parameter name and a counter as an indexer.
        /// </summary>
        /// <param name="propertyName">The propertyname for the IN statement.</param>
        /// <param name="parameters">The parameters required for the IN statement.</param>
        /// <param name="values">The values for the parameters required for the IN statement.</param>
        /// <returns></returns>
        public static string CreateInStatementForQuery(string propertyName, SqlParameterCollection parameters, params string[] values)
        {
            if (values.Length == 0)
            {
                return $"{propertyName} in ('')";
            }

            string inQueryPart = $"{propertyName} in (";
            int parameterIndex = 0;
            int lastIndex = values.Length - 1;
            string propertyNameDotsReplacedWithUnderscores = propertyName.Replace(".", "_");

            foreach (string value in values)
            {
                string parameterName = $"@{propertyNameDotsReplacedWithUnderscores}_{parameterIndex}";
                inQueryPart += parameterName;
                parameters.Add(new SqlParameter(parameterName, value));

                if (parameterIndex < lastIndex)
                {
                    inQueryPart += ",";
                }
                parameterIndex++;
            }

            return inQueryPart + ") ";
        }
    }
}
