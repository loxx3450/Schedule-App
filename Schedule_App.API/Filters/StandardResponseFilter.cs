using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Schedule_App.API.Helpers;

namespace Schedule_App.API.Filters
{
    public class StandardResponseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var objectResult = context.Result as ObjectResult;

            // By some ActionResults (204) response body is not allowed
            if (objectResult is null)
            {
                return;
            }

            var response = new ApiResponse<object>()
            {
                Success = true,
                StatusCode = objectResult.StatusCode!.Value,
                Data = objectResult.Value
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = response.StatusCode,
            };
        }
    }
}
