using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonBase.CustomExceptions
{
    public class ArgumentWithErrorCodeException : BaseBadRequestException
    {
        public ArgumentWithErrorCodeException(int errorCode, params object[] parameters) : base(errorCode, parameters)
        {
        }

        public ArgumentWithErrorCodeException(int errorCode, Exception inner, params object[] parameters) : base(errorCode, inner, parameters)
        {
        }
    }
}
