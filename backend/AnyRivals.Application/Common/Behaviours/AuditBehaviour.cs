using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyRivals.Application.Common.Behaviours;
public class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    public AuditBehaviour(ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var identifier = Guid.NewGuid();

        _logger.LogInformation("Request {ID}:{Name} started {@Request}", identifier,  requestName, request);
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        var message = "Request {ID}:{Name} completed({ElapsedMilliseconds} milliseconds)";
        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning(message, identifier, requestName, elapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation(message, identifier, requestName, elapsedMilliseconds);
        }

        return response;
    }
}
