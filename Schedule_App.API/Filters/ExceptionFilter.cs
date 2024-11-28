using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Schedule_App.API.Filters.Infrastructure;
using System.Collections;
using System.Text;

namespace Schedule_App.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var statusCode = context.Exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError,
            };

            var response = new ApiResponse<string>()
            {
                Success = false,
                StatusCode = statusCode,
                Data = context.Exception.Message,
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = response.StatusCode,
            };

            // Exception wouldn't be thrown further
            context.ExceptionHandled = true;
        }
    }
}
