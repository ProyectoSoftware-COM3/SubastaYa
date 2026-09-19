using SubastaYa.Application.Common.Filters;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetAuctionsQueryHandlerTests
    {
        private class FakeAuctionRepository : IAuctionRepository
        {
            public List<Auction> Items = new();
            public int TotalCount;
            public AuctionFilter? LastFilterReceived;

            public Task<(List<Auction> Items, int TotalCount)> GetFilteredAsync(AuctionFilter filter, CancellationToken ct = default)
            {
                LastFilterReceived = filter;
                return Task.FromResult((Items, TotalCount));
            }
            public Task<Auction?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetExpiredActiveAsync(CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddAsync(Auction auction, CancellationToken ct = default) => throw new NotImplementedException();
        }

        private static Auction CreateAuction(decimal basePrice, params decimal[] bidAmounts)
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Home", IconUrl = "icon.svg" };
            var auction = new Auction
            {
                Id = Guid.NewGuid(),
                Title = "Vintage chair",
                ImageUrl = "chair.jpg",
                Category = category,
                BasePrice = basePrice,
                EndDate = DateTime.UtcNow.AddHours(2),
                Status = AuctionStatus.Active
            };
            foreach (var amount in bidAmounts)
                auction.Bids.Add(new Bid { Amount = amount });
            return auction;
        }

        
        [Fact]
        public async Task Handle_NoBids_CurrentPriceEqualsBasePrice()
        {
            var repository = new FakeAuctionRepository { Items = { CreateAuction(basePrice: 1000) }, TotalCount = 1 };
            var handler = new GetAuctionsQueryHandler(repository);

            var result = await handler.Handle(
                new GetAuctionsQuery(null, null, null, null, AuctionSortOrder.LeastTimeRemaining), CancellationToken.None);

            Assert.Equal(1000, result.Items[0].CurrentPrice);
            Assert.Equal(0, result.Items[0].BidCount);
        }

       
        [Fact]
        public async Task Handle_WithBids_CurrentPriceEqualsHighestBid()
        {
            var repository = new FakeAuctionRepository { Items = { CreateAuction(1000, 1200, 1500, 1350) }, TotalCount = 1 };
            var handler = new GetAuctionsQueryHandler(repository);

            var result = await handler.Handle(
                new GetAuctionsQuery(null, null, null, null, AuctionSortOrder.HighestBid), CancellationToken.None);

            Assert.Equal(1500, result.Items[0].CurrentPrice);
            Assert.Equal(3, result.Items[0].BidCount);
        }

        
        [Fact]
        public async Task Handle_TranslatesQueryToAuctionFilterAndBuildsPagedResult()
        {
            var categoryId = Guid.NewGuid();
            var repository = new FakeAuctionRepository { TotalCount = 57 };
            var handler = new GetAuctionsQueryHandler(repository);

            var query = new GetAuctionsQuery(
                Status: AuctionStatus.Active, CategoryId: categoryId, MinPrice: 100, MaxPrice: 5000,
                Sort: AuctionSortOrder.HighestBid, Page: 3, PageSize: 20);

            var result = await handler.Handle(query, CancellationToken.None);

         
            var expectedFilter = new AuctionFilter(
                AuctionStatus.Active, categoryId, 100, 5000, AuctionSortOrder.HighestBid, 3, 20);
            Assert.Equal(expectedFilter, repository.LastFilterReceived);

            Assert.Equal(57, result.TotalCount);
            Assert.Equal(3, result.Page);
            Assert.Equal(20, result.PageSize);
        }
    }
}
