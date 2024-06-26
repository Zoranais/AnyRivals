﻿using AnyRivals.Application.Common.Exceptions.Abstract;
using System.Net;

namespace AnyRivals.Application.Common.Exceptions;
public class NotFoundException : RequestException
{
    private const HttpStatusCode STATUS_CODE = HttpStatusCode.NotFound;
    public NotFoundException() : base(STATUS_CODE, "Entity does not exist!")
    {
    }

    public NotFoundException(string entityName) : base(STATUS_CODE, $"{entityName} does not exist!")
    {
    }

    public NotFoundException(string entityName, string id) : base(STATUS_CODE, $"{entityName} with {id} id does not exist!")
    {
    }

    public static T ThrowIfNull<T>(T? value) where T : class
    {
        return value ?? throw new NotFoundException(nameof(value));
    }
}
