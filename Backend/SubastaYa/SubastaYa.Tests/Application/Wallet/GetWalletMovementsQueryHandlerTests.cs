using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Queries.Wallet.GetWalletMovements;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetWalletMovementsQueryHandlerTests
    {
        private class FakeWalletRepository : IWalletRepository
        {
            public Wallet? Wallet;
            public List<LedgerTransaction> Movements = new();
            public Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) => Task.FromResult(Wallet);
            public Task<List<LedgerTransaction>> GetMovementsAsync(Guid walletId, CancellationToken ct = default) => Task.FromResult(Movements);
            public Task AddMovementAsync(LedgerTransaction movement, CancellationToken ct = default) => throw new NotImplementedException();
        }

        
        [Fact]
        public async Task Handle_UserHasNoWallet_ThrowsWalletNotFoundException()
        {
            var handler = new GetWalletMovementsQueryHandler(new FakeWalletRepository { Wallet = null });

            await Assert.ThrowsAsync<WalletNotFoundException>(
                () => handler.Handle(new GetWalletMovementsQuery(Guid.NewGuid()), CancellationToken.None));
        }

        
        [Fact]
        public async Task Handle_ReturnsMovementsOrderedNewestFirst()
        {
            var wallet = new Wallet { Id = Guid.NewGuid() };
            var oldMovement = new LedgerTransaction { Id = Guid.NewGuid(), Type = LedgerTransactionType.Deposit, Amount = 500, OccurredAt = DateTime.UtcNow.AddDays(-2) };
            var recentMovement = new LedgerTransaction { Id = Guid.NewGuid(), Type = LedgerTransactionType.Hold, Amount = 200, OccurredAt = DateTime.UtcNow };
            var walletRepository = new FakeWalletRepository { Wallet = wallet, Movements = { oldMovement, recentMovement } };
            var handler = new GetWalletMovementsQueryHandler(walletRepository);

            var result = await handler.Handle(new GetWalletMovementsQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Equal(recentMovement.Id, result[0].Id);
            Assert.Equal(oldMovement.Id, result[1].Id);
        }
    }
}