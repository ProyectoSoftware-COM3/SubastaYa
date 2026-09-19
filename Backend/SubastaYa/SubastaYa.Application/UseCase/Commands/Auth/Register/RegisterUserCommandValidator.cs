using FluentValidation;

namespace SubastaYa.Application.UseCase.Commands.Auth.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
                .WithMessage("La contrasena debe tener al menos 6 caracteres.");
        }
    }
}