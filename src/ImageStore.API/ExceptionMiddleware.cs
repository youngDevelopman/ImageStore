using ImageStore.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
        _exceptionHandlers = new()
            {
                { typeof(ValidationException), HandleValidationException },

                /* All application level exceptions are handled by one method,
                  but you can easily create your own handler for each type */
                { typeof(CommentNotFoundException), HandleApplicationLevelException },
                { typeof(InvalidPasswordException), HandleApplicationLevelException },
                { typeof(PostAlreadyExistsException), HandleApplicationLevelException },
                { typeof(PostNotFoundException), HandleApplicationLevelException },
                { typeof(PostRequestNotFoundException), HandleApplicationLevelException },
                { typeof(UserAlreadyExistsException), HandleApplicationLevelException },
                { typeof(UserNotFoundException), HandleApplicationLevelException },
            };
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");

            var exceptionType = ex.GetType();

            if (_exceptionHandlers.ContainsKey(exceptionType))
            {
                await _exceptionHandlers[exceptionType].Invoke(httpContext, ex);
            }
            else
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = context.Response.StatusCode,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Detail = "Internal Server Error."
        });
    }

    private async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }

    private async Task HandleApplicationLevelException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Detail = ex.Message,
        });
    }
}