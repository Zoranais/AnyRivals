using AnyRivals.Application.Common.Exceptions.Abstract;
using System.Net;

namespace AnyRivals.Application.Common.Exceptions;
public class ValidationException : RequestException
{
    private const HttpStatusCode STATUS_CODE = HttpStatusCode.BadRequest;

    private readonly IDictionary<string, string[]> _errors;
    public IDictionary<string, string[]> Errors => _errors;

    public ValidationException(string? message, IDictionary<string, string[]> errors) : base(STATUS_CODE, message)
    {
        _errors = errors;
    }

    public ValidationException(IDictionary<string, string[]> errors) : base(STATUS_CODE, "One or more validation errors occured.")
    {
        _errors = errors;
    }
}
