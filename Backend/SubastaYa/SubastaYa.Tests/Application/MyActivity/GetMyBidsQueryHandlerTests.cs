using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.MyActivity.GetMyBids;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetMyBidsQueryHandlerTests
    {
        private class FakeBidRepository : IBidRepository
        {
            public List<Bid> MyBids = new();
            public Task AddAsync(Bid bid, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Bid?> GetHighestBidAsync(Guid auctionId, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Bid>> GetByBidderIdAsync(Guid bidderId, CancellationToken ct = default) => Task.FromResult(MyBids);
        }

        
        [Fact]
        public async Task Handle_OpenAuctionAndMyBidIsHighest_FlaggedAsCurrentlyWinning()
        {
            var auction = new Auction { Id = Guid.NewGuid(), Title = "Chair", Status = AuctionStatus.Active, BasePrice = 1000 };
            var myBid = new Bid { Auction = auction, Amount = 1200 };
            
            auction.Bids.Add(myBid);

            var handler = new GetMyBidsQueryHandler(new FakeBidRepository { MyBids = { myBid } });
            var result = await handler.Handle(new GetMyBidsQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.True(result[0].IsOpen);
            Assert.True(result[0].IsCurrentlyWinning);
            Assert.False(result[0].Won);
        }

    
        [Fact]
        public async Task Handle_FinishedAuctionAndIWon_WonTrueAndIsCurrentlyWinningFalse()
        {
            var auction = new Auction { Id = Guid.NewGuid(), Title = "Chair", Status = AuctionStatus.Finished, BasePrice = 1000 };
            var myBid = new Bid { Auction = auction, Amount = 1200 };
            auction.Bids.Add(myBid);

            var handler = new GetMyBidsQueryHandler(new FakeBidRepository { MyBids = { myBid } });
            var result = await handler.Handle(new GetMyBidsQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.False(result[0].IsOpen);
            Assert.False(result[0].IsCurrentlyWinning);
            Assert.True(result[0].Won);
        }

        
        [Fact]
        public async Task Handle_FinishedAuctionAndSomeoneElseOutbidMe_WonFalse()
        {
            var auction = new Auction { Id = Guid.NewGuid(), Title = "Chair", Status = AuctionStatus.Finished, BasePrice = 1000 };
            var myBid = new Bid { Auction = auction, Amount = 800 };
            var winningBidFromSomeoneElse = new Bid { Auction = auction, Amount = 1200 };
            auction.Bids.Add(myBid);
            auction.Bids.Add(winningBidFromSomeoneElse);

            var handler = new GetMyBidsQueryHandler(new FakeBidRepository { MyBids = { myBid } });
            var result = await handler.Handle(new GetMyBidsQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Equal(800, result[0].MyLastBid);
            Assert.False(result[0].Won);
            Assert.False(result[0].IsCurrentlyWinning);
        }
    }
}
