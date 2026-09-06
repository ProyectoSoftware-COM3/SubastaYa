using FluentValidation;
using MediatR;

namespace SubastaYa.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
            => _validators = validators;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var failures = new List<FluentValidation.Results.ValidationFailure>();
            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(request, cancellationToken);
                failures.AddRange(result.Errors);
            }

            if (failures.Count > 0)
                throw new ValidationException(failures);

            return await next();
        }
    }
}