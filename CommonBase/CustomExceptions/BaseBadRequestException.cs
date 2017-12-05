using System;
using System.Net;

namespace CommonBase.CustomExceptions
{
    public abstract class BaseBadRequestException : BaseException
    {
        protected BaseBadRequestException(int errorCode, params object[] parameters) : base(errorCode, parameters)
        {
            HttpStatusCode = HttpStatusCode.BadRequest;
        }

        protected BaseBadRequestException(int errorCode, Exception inner, params object[] parameters) : base(errorCode, inner, parameters)
        {
            HttpStatusCode = HttpStatusCode.BadRequest;
        }
    }
}
