using SubastaYa.Application.Common.Filters;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionBids;
using SubastaYa.Domain.Entities;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetAuctionBidsQueryHandlerTests
    {
        private class FakeAuctionRepository : IAuctionRepository
        {
            public Auction? Auction;
            public Task<(List<Auction> Items, int TotalCount)> GetFilteredAsync(AuctionFilter filter, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default) => Task.FromResult(Auction);
            public Task<List<Auction>> GetExpiredActiveAsync(CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddAsync(Auction auction, CancellationToken ct = default) => throw new NotImplementedException();
        }

        
        [Fact]
        public async Task Handle_AuctionDoesNotExist_ThrowsAuctionNotFoundException()
        {
            var handler = new GetAuctionBidsQueryHandler(new FakeAuctionRepository { Auction = null });

            await Assert.ThrowsAsync<AuctionNotFoundException>(
                () => handler.Handle(new GetAuctionBidsQuery(Guid.NewGuid()), CancellationToken.None));
        }

        
        [Fact]
        public async Task Handle_ReturnsHistoryOrderedByDateWithAnonymizedAlias()
        {
            var auction = new Auction { Id = Guid.NewGuid() };
            var firstBid = new Bid { Id = Guid.NewGuid(), Amount = 1000, PlacedAt = DateTime.UtcNow.AddMinutes(-10), Bidder = new User { Name = "Ana" } };
            var secondBid = new Bid { Id = Guid.NewGuid(), Amount = 1200, PlacedAt = DateTime.UtcNow.AddMinutes(-5), Bidder = new User { Name = "Bruno" } };
            
            auction.Bids.Add(secondBid);
            auction.Bids.Add(firstBid);

            var handler = new GetAuctionBidsQueryHandler(new FakeAuctionRepository { Auction = auction });
            var result = await handler.Handle(new GetAuctionBidsQuery(auction.Id), CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.Equal(firstBid.Id, result[0].Id);
            Assert.Equal("A***a", result[0].BidderAlias);
            Assert.Equal(secondBid.Id, result[1].Id);
            Assert.Equal("B***o", result[1].BidderAlias);
        }
    }
}
