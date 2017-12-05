using CommonBase.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Core
{
    internal enum TraceWriterWritingTypes
    {
        Information = 0,
        Warning,
        Error
    }

    internal class DbOperationTraceWriter
    {
        private readonly string _databaseName;

        internal DbOperationTraceWriter(string databaseName)
        {
            _databaseName = databaseName;
        }

        /// <summary>
        /// Writes a message to the tracer.
        /// </summary>
        /// <param name="writerWritingType">The type of the message.</param>
        /// <param name="message">The message that must be written to the tracer.</param>
        /// <param name="collectionName">The collection name for which a message is written to the tracer.</param>
        /// <param name="arguments">The arguments that must be formatted into the message.</param>
        internal void WriteTrace(TraceWriterWritingTypes writerWritingType, string message, string collectionName, params object[] arguments)
        {
            var names = new object[] { _databaseName, collectionName };
            arguments = names.Concat(arguments).ToArray();
            message = string.Format(message, arguments);

            switch (writerWritingType)
            {
                case TraceWriterWritingTypes.Information: Trace.TraceInformation(message); break;
                case TraceWriterWritingTypes.Warning: Trace.TraceWarning(message); break;
                case TraceWriterWritingTypes.Error: Trace.TraceError(message); break;
                default:
                    Trace.WriteLine(message);
                    Trace.Flush();
                    break;
            }
        }

        /// <summary>
        /// Writes a message of type information to the tracer.
        /// </summary>
        /// <param name="informationCode">The code linked to the message that must be written to the tracer.</param>
        /// <param name="collectionName">The collection name for which a message is written to the tracer.</param>
        /// <param name="arguments">The arguments that must be formatted into the message.</param>
        internal void TraceInformation(int informationCode, string collectionName, params object[] arguments)
        {
            string message = ErrorCodes.GetMessageByCode(informationCode);
            WriteTrace(TraceWriterWritingTypes.Warning, message, collectionName, arguments);
        }

        /// <summary>
        /// Writes a message of type information to the tracer.
        /// </summary>
        /// <param name="warningCode">The code linked to the message that must be written to the tracer.</param>
        /// <param name="collectionName">The collection name for which a message is written to the tracer.</param>
        /// <param name="arguments">The arguments that must be formatted into the message.</param>
        internal void TraceWarning(int warningCode, string collectionName, params object[] arguments)
        {
            string message = ErrorCodes.GetMessageByCode(warningCode);
            WriteTrace(TraceWriterWritingTypes.Warning, message, collectionName, arguments);
        }

        /// <summary>
        /// Writes a message of type information to the tracer.
        /// </summary>
        /// <param name="errorCode">The code linked to the message that must be written to the tracer.</param>
        /// <param name="collectionName">The collection name for which a message is written to the tracer.</param>
        /// <param name="arguments">The arguments that must be formatted into the message.</param>
        internal void TraceError(int errorCode, string collectionName, params object[] arguments)
        {
            string message = ErrorCodes.GetMessageByCodeWithCode(errorCode);
            WriteTrace(TraceWriterWritingTypes.Error, message, collectionName, arguments);
        }

        /// <summary>
        /// Writes a message of type information to the tracer.
        /// </summary>
        /// <param name="message">The message that must be written to the tracer.</param>
        /// <param name="collectionName">The collection name for which a message is written to the tracer.</param>
        /// <param name="arguments">The arguments that must be formatted into the message.</param>
        internal void TraceError(string message, string collectionName, params object[] arguments)
        {
            WriteTrace(TraceWriterWritingTypes.Error, message, collectionName, arguments);
        }
    }
}
