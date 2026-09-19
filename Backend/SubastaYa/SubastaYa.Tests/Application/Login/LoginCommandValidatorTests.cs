using FluentValidation.TestHelper;
using SubastaYa.Application.UseCase.Commands.Auth.Login;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class LoginCommandValidatorTests
    {
        
        [Fact]
        public void Login_RejectsEmptyFields()
        {
            var validator = new LoginCommandValidator();

            validator.TestValidate(new LoginCommand("", "")).ShouldHaveValidationErrorFor(x => x.Email);
        }
    }
}
