using CommonBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CommonBase.CustomExceptions
{
    public abstract class BaseException : Exception
    {
        public int ErrorCode { get; }

        public object[] Parameters { get; }

        protected HttpStatusCode HttpStatusCode { get; set; }

        protected BaseException(int errorCode, params object[] parameters) : base(string.Format(ErrorCodes.GetMessageByCodeWithCode(errorCode), parameters))
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        protected BaseException(int errorCode, Exception inner, params object[] parameters) : base(string.Format(ErrorCodes.GetMessageByCodeWithCode(errorCode)), inner)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        protected BaseException(int errorCode, SerializationInfo info, StreamingContext context, params object[] parameters) : base(info, context)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }

        /// <summary>
        /// Creates a http resonse message from the request.
        /// </summary>
        /// <param name="request">The request which is used for creating a response.</param>
        /// <returns>The response message.</returns>
        public ContentResult CreateResponseMessageResult(HttpRequest request)
        {
            var responseMsg = GetResponseMessage();
            return new ContentResult
            {
                Content = responseMsg.Message,
                ContentType = "text/plain",
                StatusCode = (int) HttpStatusCode
            };

        }

        /// <summary>
        /// Creates and returns an exception error message meant for the front end to inform.
        /// </summary>
        /// <returns>The response message meant for the front end.</returns>
        private ResponseMessage GetResponseMessage()
        {
            return new ResponseMessage
            {
                ErrorCode = ErrorCode,
                Message = GetMessage(),
                Parameters = GetParameters()
            };
        }

        /// <summary>
        /// Returns a message based on some settings. For Example, Internal Server Error messages should not be returned to the 
        /// front end.
        /// </summary>
        /// <returns>Returns a message based on settings.</returns>
        private string GetMessage(bool messageIncludesErrorCode = false)
        {
            if (HttpStatusCode == HttpStatusCode.InternalServerError)
            {
                return "Internal Server Error";
            }
            if (!messageIncludesErrorCode)
            {
                return string.Format(ErrorCodes.GetMessageByCode(ErrorCode), Parameters);
            }
            if (string.IsNullOrWhiteSpace(Message))
            {
                return string.Format(ErrorCodes.GetMessageByCodeWithCode(ErrorCode), Parameters);
            }
            return Message;
        }

        /// <summary>
        /// Returns a list of parameters if the HttpStatusCode allows it. 
        /// </summary>
        /// <returns>List of parameter values.</returns>
        private string[] GetParameters()
        {
            if (HttpStatusCode == HttpStatusCode.InternalServerError)
            {
                return null;
            }
            return Parameters.Select(parameterValue => parameterValue.ToString()).ToArray();
        }
    }
}
