
using FluentValidation.TestHelper;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetAuctionsQueryValidatorTests
    {
        
        [Fact]
        public void GetAuctions_RejectsPageBelowOneAndPageSizeOutOfRange()
        {
            var validator = new GetAuctionsQueryValidator();

            validator.TestValidate(new GetAuctionsQuery(null, null, null, null, AuctionSortOrder.LeastTimeRemaining, Page: 0))
                .ShouldHaveValidationErrorFor(x => x.Page);
            validator.TestValidate(new GetAuctionsQuery(null, null, null, null, AuctionSortOrder.LeastTimeRemaining, PageSize: 500))
                .ShouldHaveValidationErrorFor(x => x.PageSize);
        }

        
        [Fact]
        public void GetAuctions_RejectsMaxPriceLowerThanMinPrice()
        {
            var validator = new GetAuctionsQueryValidator();

            var query = new GetAuctionsQuery(
                Status: null, CategoryId: null, MinPrice: 5000, MaxPrice: 1000,
                Sort: AuctionSortOrder.LeastTimeRemaining);

            validator.TestValidate(query).ShouldHaveValidationErrorFor(x => x.MaxPrice);
        }
    }
}