using AnyRivals.Application.Common.Exceptions.Abstract;
using System.Net;

namespace AnyRivals.Application.Common.Exceptions;
public class BadRequestException : RequestException
{
    private const HttpStatusCode STATUS_CODE = HttpStatusCode.BadRequest;
    public BadRequestException(string? message) : base(STATUS_CODE, message)
    {
    }
}
