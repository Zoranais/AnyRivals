﻿using AnyRivals.WebAPI.ExceptionHandlers.Abstract;
using ValidationException = AnyRivals.Application.Common.Exceptions.ValidationException;

namespace AnyRivals.WebAPI.ExceptionHandlers;

public class ValidationExceptionHandler : BaseExceptionHandler
{
    private readonly ILogger<RequestExceptionHandler> _logger;

    public ValidationExceptionHandler(ILogger<RequestExceptionHandler> logger)
    {
        _logger = logger;
    }

    public override async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            _logger.LogError(
                exception,
                "Validation error occured: {Message} {@Errors} {@Exception}",
                exception.Message,
                validationException.Errors,
                validationException);
            await HandleException(httpContext, exception, validationException.Errors);

            return true;
        }

        return false;
    }
}
