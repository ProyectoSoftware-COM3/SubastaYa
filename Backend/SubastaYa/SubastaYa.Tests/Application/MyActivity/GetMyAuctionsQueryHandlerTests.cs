
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.MyActivity.GetMyAuctions;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetMyAuctionsQueryHandlerTests
    {
        private class FakeAuctionRepository : IAuctionRepository
        {
            public List<Auction> MyAuctions = new();
            public Task<(List<Auction> Items, int TotalCount)> GetFilteredAsync(SubastaYa.Application.Common.Filters.AuctionFilter filter, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetExpiredActiveAsync(CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default) => Task.FromResult(MyAuctions);
            public Task AddAsync(Auction auction, CancellationToken ct = default) => throw new NotImplementedException();
        }

       
        [Fact]
        public async Task Handle_UnsoldAuction_RevenueIsZeroEvenWithBasePrice()
        {
            var auction = new Auction { Id = Guid.NewGuid(), Title = "Chair", Status = AuctionStatus.Unsold, BasePrice = 1000 };
            var handler = new GetMyAuctionsQueryHandler(new FakeAuctionRepository { MyAuctions = { auction } });

            var result = await handler.Handle(new GetMyAuctionsQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Equal(0, result[0].Revenue);
            Assert.Equal(0, result[0].BidCount);
        }

        
        [Fact]
        public async Task Handle_FinishedAuctionWithBids_RevenueEqualsHighestBid()
        {
            var auction = new Auction { Id = Guid.NewGuid(), Title = "Chair", Status = AuctionStatus.Finished, BasePrice = 1000 };
            auction.Bids.Add(new Bid { Amount = 1300 });
            auction.Bids.Add(new Bid { Amount = 1600 });
            var handler = new GetMyAuctionsQueryHandler(new FakeAuctionRepository { MyAuctions = { auction } });

            var result = await handler.Handle(new GetMyAuctionsQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Equal(1600, result[0].Revenue);
            Assert.Equal(2, result[0].BidCount);
        }
    }
}
