using FluentValidation;
using MediatR;
using System.Linq;
using ValidationException = AnyRivals.Application.Common.Exceptions.ValidationException;

namespace AnyRivals.Application.Common.Behaviours;
internal class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var isValid = Array.TrueForAll(validationResults, r => r.IsValid);

            if (!isValid)
            {
                var errors = validationResults
                    .Where(x =>  !x.IsValid)
                    .SelectMany(x => x.Errors)
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

                throw new ValidationException(errors);
            }
        }

        return await next();
    }
}
