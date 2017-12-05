using CommonBase.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CommonBase.Controllers
{
    public class EdmlApiController : Controller
    {
        /// <summary>
        /// Executes the given function and handles the exceptions by catching it and transforming it 
        /// to a bad request. If no exception is caught, the result of the function will be returned.
        /// </summary>
        /// <param name="execute">The function that needs to be executed.</param>
        /// <returns>If no exception is caught, the result of the executed function.</returns> 
        protected async Task<IActionResult> GuardedExecute(Func<Task<IActionResult>> execute)
        {
            try
            {
                return await execute();
            }
            catch (BaseException exception)
            {
                Trace.TraceError(exception.Message);
                //Todo: telemetry
                return exception.CreateResponseMessageResult(Request);
            }
        }
    }
}
