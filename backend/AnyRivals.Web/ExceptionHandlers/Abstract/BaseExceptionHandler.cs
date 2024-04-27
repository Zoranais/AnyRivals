using Microsoft.AspNetCore.Diagnostics;
using AnyRivals.Application.Common.Exceptions.Abstract;
using AnyRivals.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace AnyRivals.WebAPI.ExceptionHandlers.Abstract;

public abstract class BaseExceptionHandler : IExceptionHandler
{
    public abstract ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken);


    protected private virtual Task HandleException(HttpContext context, Exception exception, IDictionary<string, string[]>? errors = null)
    {
        var statusCode = GetExceptionStatusCode(exception);
        var message = statusCode != HttpStatusCode.InternalServerError ? exception.Message : "Something went wrong";

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(SerializeErrorDetails(message, errors));
    }

    protected private string SerializeErrorDetails(string message, IDictionary<string, string[]>? errors = null)
    {
        var details = new ErrorDetails(message, errors);

        return JsonSerializer.Serialize(details);
    }

    protected private HttpStatusCode GetExceptionStatusCode(Exception exception)
    {
        if (exception is RequestException requestException)
        {
            return requestException.StatusCode;
        }
        else
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
