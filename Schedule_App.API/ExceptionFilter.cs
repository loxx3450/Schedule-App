using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Schedule_App.API
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var statusCode = context.Exception switch
            {
                ArgumentException => 400,
                KeyNotFoundException => 404,
                _ => 500
            };

            ProblemDetails details = new ProblemDetails()
            {
                Status = statusCode,
                Title = context.Exception.Message,
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }
    }
}
