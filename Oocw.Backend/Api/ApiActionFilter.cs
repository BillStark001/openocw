
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Oocw.Backend.Api;

public class ApiActionFilter(ILogger<ApiActionFilter> logger) : IActionFilter
{
    private readonly ILogger<ApiActionFilter> _logger = logger;

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            // an error occurs

            var apiException = context.Exception as ApiException;

            if (apiException == null)
            {
                // the exception type is unknown
                _logger.LogError(context.Exception, "An unhandled exception occurred.");
            }

            context.Result = new ObjectResult(apiException != null
                ? new ApiResult
                {
                    Code = apiException.Code,
                    Message = !string.IsNullOrWhiteSpace(apiException.Message) ? apiException.Message : null
                } : new ApiResult { Code = ApiResult.CODE_INTERNAL_ERROR }
            )
            {
                StatusCode = apiException != null
                    ? apiException.StatusCode 
                    : (int)HttpStatusCode.InternalServerError
            };
            context.ExceptionHandled = true;

            return;
        }
        // else everything is normal

        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is ApiResult)
            {
                // do nothing, just return
                return;
            }
            // wrap it in an api response object
            var v = objectResult.Value;
            objectResult.Value = new ApiResult { Data = v };
        }

        else if (context.Result is StatusCodeResult statusCodeResult) {
            context.Result = new ObjectResult(new ApiResult()) {
                StatusCode = statusCodeResult.StatusCode
            };
        }

        else if (context.Result is EmptyResult) {
            context.Result = new ObjectResult(new ApiResult());
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // do nothing
    }
}