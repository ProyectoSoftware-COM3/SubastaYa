using FluentValidation.TestHelper;
using SubastaYa.Application.UseCase.Commands.Auctions.CreateAuction;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class CreateAuctionCommandValidatorTests
    {
        
        [Fact]
        public void CreateAuction_RejectsEndDateBeforeOrEqualToStartDate()
        {
            var validator = new CreateAuctionCommandValidator();
            var start = DateTime.UtcNow;
            var sellerId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var invalidCommand = new CreateAuctionCommand(sellerId, "T", "D", "img.jpg", categoryId, 100, 10, start, start.AddMinutes(-5));
            validator.TestValidate(invalidCommand).ShouldHaveValidationErrorFor(x => x.EndDate);

            var validCommand = new CreateAuctionCommand(sellerId, "T", "D", "img.jpg", categoryId, 100, 10, start, start.AddHours(2));
            validator.TestValidate(validCommand).ShouldNotHaveValidationErrorFor(x => x.EndDate);
        }
    }
}