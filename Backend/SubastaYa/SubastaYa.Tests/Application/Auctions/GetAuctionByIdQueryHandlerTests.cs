using SubastaYa.Application.Common.Filters;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionById;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetAuctionByIdQueryHandlerTests
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

        private static Auction CreateAuctionWithBids(Guid userId, out Bid myBid)
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Home", IconUrl = "icon.svg" };
            var auction = new Auction
            {
                Id = Guid.NewGuid(),
                Title = "Vintage chair",
                Description = "d",
                ImageUrl = "chair.jpg",
                Category = category,
                BasePrice = 1000,
                MinIncrement = 100,
                StartDate = DateTime.UtcNow.AddHours(-1),
                EndDate = DateTime.UtcNow.AddHours(1),
                Status = AuctionStatus.Active
            };
            myBid = new Bid { Id = Guid.NewGuid(), BidderId = userId, Amount = 1200, PlacedAt = DateTime.UtcNow, Bidder = new User { Name = "Ana" } };
            auction.Bids.Add(myBid);
            return auction;
        }

        
        [Fact]
        public async Task Handle_AuctionDoesNotExist_ThrowsAuctionNotFoundException()
        {
            var handler = new GetAuctionByIdQueryHandler(new FakeAuctionRepository { Auction = null });

            await Assert.ThrowsAsync<AuctionNotFoundException>(
                () => handler.Handle(new GetAuctionByIdQuery(Guid.NewGuid()), CancellationToken.None));
        }

        
        [Fact]
        public async Task Handle_NoLoggedInUser_IsCurrentUserLeadingIsNull()
        {
            var auction = CreateAuctionWithBids(Guid.NewGuid(), out _);
            var handler = new GetAuctionByIdQueryHandler(new FakeAuctionRepository { Auction = auction });

            var result = await handler.Handle(new GetAuctionByIdQuery(auction.Id, CurrentUserId: null), CancellationToken.None);

            Assert.Null(result.IsCurrentUserLeading);
        }

   
        [Fact]
        public async Task Handle_LoggedInUserIsLeading_IsCurrentUserLeadingTrue()
        {
            var userId = Guid.NewGuid();
            var auction = CreateAuctionWithBids(userId, out _);
            var handler = new GetAuctionByIdQueryHandler(new FakeAuctionRepository { Auction = auction });

            var result = await handler.Handle(new GetAuctionByIdQuery(auction.Id, userId), CancellationToken.None);

            Assert.True(result.IsCurrentUserLeading);
            Assert.Equal(1200, result.CurrentPrice);
            Assert.Equal(1300, result.SuggestedNextBid);
        }

        [Fact]
        public async Task Handle_LoggedInUserWasOutbid_IsCurrentUserLeadingFalse()
        {
            var userId = Guid.NewGuid();
            var auction = CreateAuctionWithBids(userId, out var myBid);
           
            auction.Bids.Add(new Bid { Id = Guid.NewGuid(), BidderId = Guid.NewGuid(), Amount = 1400, PlacedAt = DateTime.UtcNow, Bidder = new User { Name = "Bruno" } });
            var handler = new GetAuctionByIdQueryHandler(new FakeAuctionRepository { Auction = auction });

            var result = await handler.Handle(new GetAuctionByIdQuery(auction.Id, userId), CancellationToken.None);

            Assert.False(result.IsCurrentUserLeading);
            Assert.Equal(1400, result.CurrentPrice);
        }
    }
}
