using SubastaYa.Application.Common.Filters;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Commands.Auctions.CreateAuction;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class CreateAuctionCommandHandlerTests
    {
        private class FakeAuctionRepository : IAuctionRepository
        {
            public Auction? LastAuctionAdded;
            public Task<(List<Auction> Items, int TotalCount)> GetFilteredAsync(AuctionFilter filter, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Auction?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetExpiredActiveAsync(CancellationToken ct = default) => throw new NotImplementedException();
            public Task<List<Auction>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddAsync(Auction auction, CancellationToken ct = default) { LastAuctionAdded = auction; return Task.CompletedTask; }
        }

        private class FakeCategoryRepository : ICategoryRepository
        {
            public Category? Category;
            public Task<List<Category>> GetAllAsync(CancellationToken ct = default) => throw new NotImplementedException();
            public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult(Category);
        }

        private class FakeUnitOfWork : IUnitOfWork
        {
            public Task BeginTransactionAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task RollbackAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(1);
        }

        private static CreateAuctionCommand BuildCommand(DateTime startDate, Guid categoryId) => new(
            SellerId: Guid.NewGuid(), Title: "T", Description: "D", ImageUrl: "img.jpg",
            CategoryId: categoryId, BasePrice: 1000, MinIncrement: 100,
            StartDate: startDate, EndDate: startDate.AddHours(2));

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsCategoryNotFoundException()
        {
            var handler = new CreateAuctionCommandHandler(new FakeAuctionRepository(), new FakeCategoryRepository { Category = null }, new FakeUnitOfWork());

            var command = BuildCommand(DateTime.UtcNow.AddHours(1), Guid.NewGuid());

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        
        [Fact]
        public async Task Handle_StartDateInTheFuture_CreatesAuctionWithScheduledStatus()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Home", IconUrl = "icon.svg" };
            var auctionRepository = new FakeAuctionRepository();
            var handler = new CreateAuctionCommandHandler(auctionRepository, new FakeCategoryRepository { Category = category }, new FakeUnitOfWork());

            var command = BuildCommand(DateTime.UtcNow.AddDays(1), category.Id);
            await handler.Handle(command, CancellationToken.None);

            Assert.Equal(AuctionStatus.Scheduled, auctionRepository.LastAuctionAdded!.Status);
        }

        
        [Fact]
        public async Task Handle_StartDateNowOrInThePast_CreatesAuctionWithActiveStatus()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Home", IconUrl = "icon.svg" };
            var auctionRepository = new FakeAuctionRepository();
            var handler = new CreateAuctionCommandHandler(auctionRepository, new FakeCategoryRepository { Category = category }, new FakeUnitOfWork());

            var command = BuildCommand(DateTime.UtcNow.AddMinutes(-5), category.Id);
            var id = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(AuctionStatus.Active, auctionRepository.LastAuctionAdded!.Status);
            Assert.Equal(id, auctionRepository.LastAuctionAdded.Id);
        }
    }
}
