
using FluentValidation.TestHelper;
using SubastaYa.Application.UseCase.Commands.Wallet.DepositFunds;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class DepositFundsCommandValidatorTests
    {
        
        [Fact]
        public void DepositFunds_RejectsNegativeOrZeroAmount()
        {
            var validator = new DepositFundsCommandValidator();
            var userId = Guid.NewGuid();

            validator.TestValidate(new DepositFundsCommand(userId, -100)).ShouldHaveValidationErrorFor(x => x.Amount);
            validator.TestValidate(new DepositFundsCommand(userId, 0)).ShouldHaveValidationErrorFor(x => x.Amount);
            validator.TestValidate(new DepositFundsCommand(userId, 500)).ShouldNotHaveValidationErrorFor(x => x.Amount);
        }
    }
}
