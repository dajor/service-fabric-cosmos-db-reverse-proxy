using CommonBase.CustomExceptions;
using CommonBase.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Core
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argValue">Argument value to test.</param>
        /// <param name="argName">Name of the argument being tested.</param>
        public static void ArgumentNotNull(string argName, object argValue)
        {
            if (argValue == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is null or empty.</exception>
        /// <param name="argValue">Argument value to check.</param>
        /// <param name="argName">Name of argument being checked.</param>
        public static void ArgumentNotNullOrEmpty(string argName, string argValue)
        {
            if (string.IsNullOrEmpty(argValue))
            {
                throw new ArgumentWithErrorCodeException(ErrorCodes.ArgumentNullOrEmpty, argValue);
            }
        }

        /// <summary>
        /// Throws an exception if the tested IEnumerable argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if IEnumerable value is null.</exception>
        /// <exception cref="ArgumentException">Thrown if IEnumerable value is empty.</exception>
        /// <param name="argValue">Argument value to check.</param>
        /// <param name="argName">Name of argument being checked.</param>
        public static void ArgumentNotNullOrEmpty(string argName, IEnumerable argValue)
        {
            //No null check for argValue required as long as the overload with string argValue exists. Because
            //when called with argValue is null, the overload with string argValue will always be called.

            if (!argValue.GetEnumerator().MoveNext())
            {
                throw new ArgumentWithErrorCodeException(ErrorCodes.ArgumentNullOrEmpty, argValue);
            }
        }

        /// <summary>
        /// Throws an exception if the tested object is not of type T.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if object is not of type T.</exception>
        /// <param name="argValue">Argument value to check.</param>
        /// <param name="argName">Name of argument being checked.</param>
        public static void ArgumentOfType<T>(string argName, object argValue)
        {
            ArgumentNotNull(argName, argValue);

            Type typeParameterType = typeof(T);
            if (typeParameterType != argValue.GetType())
            {
                throw new ArgumentWithErrorCodeException(ErrorCodes.ArgumentWrongType, argName, typeParameterType.Name, argValue.GetType().Name);
            }
        }
    }
}
