﻿using AnyRivals.Application.Common.Exceptions.Abstract;
using AnyRivals.WebAPI.ExceptionHandlers.Abstract;

namespace AnyRivals.WebAPI.ExceptionHandlers;

public class RequestExceptionHandler : BaseExceptionHandler
{
    private readonly ILogger<RequestExceptionHandler> _logger;

    public RequestExceptionHandler(ILogger<RequestExceptionHandler> logger)
    {
        _logger = logger;
    }

    public override async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is RequestException requestException)
        {
            _logger.LogError(requestException, "Request error occured: {Message}", exception.Message);
            await HandleException(httpContext, requestException);

            return true;
        }

        return false;
    }
}
