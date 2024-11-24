using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Schedule_App.API.Helpers;

namespace Schedule_App.API.Filters
{
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var response = new ApiResponse<Dictionary<string, List<string>>>()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Data = []
                };

                foreach (var entry in context.ModelState.AsEnumerable())
                {
                    var errors = new List<string>();

                    // Collecting errors for every Entry(Field of DTO)
                    foreach (var error in entry.Value!.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }

                    if (errors.Count > 0)
                    {
                        response.Data.Add(entry.Key, errors);
                    }
                }

                // Returning custom ObjectResult
                context.Result = new ObjectResult(response);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
