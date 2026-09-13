using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Commands.Wallet.DepositFunds;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class DepositFundsCommandHandlerTests
    {
        private class FakeWalletRepository : IWalletRepository
        {
            public Wallet? Wallet;
            public LedgerTransaction? LastMovementAdded;
            public Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) => Task.FromResult(Wallet);
            public Task<List<LedgerTransaction>> GetMovementsAsync(Guid walletId, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddMovementAsync(LedgerTransaction movement, CancellationToken ct = default) { LastMovementAdded = movement; return Task.CompletedTask; }
        }

        private class FakeAuditLogRepository : IAuditLogRepository
        {
            public int CallCount;
            public string? LastAction;
            public Task AddAsync(AuditLog entry, CancellationToken ct = default) { CallCount++; LastAction = entry.Action; return Task.CompletedTask; }
        }

        private class FakeUnitOfWork : IUnitOfWork
        {
            public Task BeginTransactionAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task RollbackAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(1);
        }

   
        [Fact]
        public async Task Handle_UserHasNoWallet_ThrowsWalletNotFoundExceptionAndSkipsAuditLog()
        {
            var auditLogRepository = new FakeAuditLogRepository();
            var handler = new DepositFundsCommandHandler(new FakeWalletRepository { Wallet = null }, auditLogRepository, new FakeUnitOfWork());

            await Assert.ThrowsAsync<WalletNotFoundException>(
                () => handler.Handle(new DepositFundsCommand(Guid.NewGuid(), 500), CancellationToken.None));
            Assert.Equal(0, auditLogRepository.CallCount);
        }

        
        [Fact]
        public async Task Handle_SuccessfulDeposit_UpdatesBalanceRecordsMovementAndAuditLog()
        {
            var wallet = new Wallet { Id = Guid.NewGuid(), TotalBalance = 1000, HeldBalance = 300, AvailableBalance = 700 };
            var walletRepository = new FakeWalletRepository { Wallet = wallet };
            var auditLogRepository = new FakeAuditLogRepository();
            var handler = new DepositFundsCommandHandler(walletRepository, auditLogRepository, new FakeUnitOfWork());

            var result = await handler.Handle(new DepositFundsCommand(Guid.NewGuid(), 500), CancellationToken.None);

            Assert.Equal(1500, result.TotalBalance);
            Assert.Equal(300, result.HeldBalance);
            Assert.Equal(1200, result.AvailableBalance);
            Assert.Equal(LedgerTransactionType.Deposit, walletRepository.LastMovementAdded!.Type);
            Assert.Equal(500, walletRepository.LastMovementAdded.Amount);
            Assert.Equal(1, auditLogRepository.CallCount);
            Assert.Equal("AcreditacionManual", auditLogRepository.LastAction);
        }
    }
}
