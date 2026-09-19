using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Queries.Wallet.GetWalletBalance;
using SubastaYa.Domain.Entities;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class GetWalletBalanceQueryHandlerTests
    {
        private class FakeWalletRepository : IWalletRepository
        {
            public Wallet? Wallet;
            public Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) => Task.FromResult(Wallet);
            public Task<List<LedgerTransaction>> GetMovementsAsync(Guid walletId, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddMovementAsync(LedgerTransaction movement, CancellationToken ct = default) => throw new NotImplementedException();
        }

        
        [Fact]
        public async Task Handle_UserHasNoWallet_ThrowsWalletNotFoundException()
        {
            var handler = new GetWalletBalanceQueryHandler(new FakeWalletRepository { Wallet = null });

            await Assert.ThrowsAsync<WalletNotFoundException>(
                () => handler.Handle(new GetWalletBalanceQuery(Guid.NewGuid()), CancellationToken.None));
        }

       
        [Fact]
        public async Task Handle_ReturnsTheWalletsThreeBalances()
        {
            var wallet = new Wallet { TotalBalance = 5000, HeldBalance = 1200, AvailableBalance = 3800 };
            var handler = new GetWalletBalanceQueryHandler(new FakeWalletRepository { Wallet = wallet });

            var result = await handler.Handle(new GetWalletBalanceQuery(Guid.NewGuid()), CancellationToken.None);

            Assert.Equal(5000, result.TotalBalance);
            Assert.Equal(1200, result.HeldBalance);
            Assert.Equal(3800, result.AvailableBalance);
        }
    }
}
