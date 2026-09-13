using FluentValidation.TestHelper;
using SubastaYa.Application.UseCase.Commands.Auth.Register;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class RegisterUserCommandValidatorTests
    {
        
        [Fact]
        public void RegisterUser_RejectsInvalidEmailAndShortPassword()
        {
            var validator = new RegisterUserCommandValidator();

            validator.TestValidate(new RegisterUserCommand("not-an-email", "Ana", "123456"))
                .ShouldHaveValidationErrorFor(x => x.Email);
            validator.TestValidate(new RegisterUserCommand("ana@test.com", "Ana", "123"))
                .ShouldHaveValidationErrorFor(x => x.Password);
        }
    }
}
