using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using SubastaYa.Application.Exceptions;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.BackgroundJobs
{
    public class AuctionClosingWorker : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionClosingWorker> _logger;

        public AuctionClosingWorker(IServiceScopeFactory scopeFactory, ILogger<AuctionClosingWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CloseExpiredAuctionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cerrando subastas vencidas");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }

        private async Task CloseExpiredAuctionsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
            var walletRepository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();
            var auditLogRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
            var auctionNotifier = scope.ServiceProvider.GetRequiredService<IAuctionNotifier>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var expiredAuctions = await auctionRepository.GetExpiredActiveAsync(ct);

            foreach (var auction in expiredAuctions)
            {
                await CloseOneAuctionAsync(auction, walletRepository, auditLogRepository, auctionNotifier, unitOfWork, ct);
            }
        }

        private async Task CloseOneAuctionAsync(
            Auction auction, IWalletRepository walletRepository, IAuditLogRepository auditLogRepository,
            IAuctionNotifier auctionNotifier, IUnitOfWork unitOfWork, CancellationToken ct)
        {
            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var winningBid = GetWinningBid(auction);

                if (winningBid is null)
                    auction.Status = AuctionStatus.Unsold;
                else
                    await LiquidateAuctionAsync(auction, winningBid, walletRepository, ct);

                await LogClosureAuditAsync(auction, auditLogRepository, ct);

                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);

                await auctionNotifier.NotifyAuctionClosedAsync(auction.Id, winningBid is not null, ct);
            }
            catch (ConcurrencyConflictException)
            {
                await unitOfWork.RollbackAsync(ct);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                _logger.LogError(ex, "No se pudo cerrar la subasta {AuctionId}, se reintentara en el proximo ciclo", auction.Id);
            }
        }

        private static Bid? GetWinningBid(Auction auction)
            => auction.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();

        private static async Task LiquidateAuctionAsync(Auction auction, Bid winningBid, IWalletRepository walletRepository, CancellationToken ct)
        {
            var buyerWallet = await walletRepository.GetByUserIdAsync(winningBid.BidderId, ct)
                ?? throw new InvalidOperationException($"No se encontro la billetera del comprador ganador (userId {winningBid.BidderId}) para la subasta {auction.Id}.");
            var sellerWallet = await walletRepository.GetByUserIdAsync(auction.SellerId, ct)
                ?? throw new InvalidOperationException($"No se encontro la billetera del vendedor (userId {auction.SellerId}) para la subasta {auction.Id}.");

            buyerWallet.TotalBalance -= winningBid.Amount;
            buyerWallet.HeldBalance -= winningBid.Amount;
            buyerWallet.AvailableBalance = buyerWallet.TotalBalance - buyerWallet.HeldBalance;

            sellerWallet.TotalBalance += winningBid.Amount;
            sellerWallet.AvailableBalance = sellerWallet.TotalBalance - sellerWallet.HeldBalance;

            await walletRepository.AddMovementAsync(new LedgerTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = buyerWallet.Id,
                Type = LedgerTransactionType.Payment,
                Amount = winningBid.Amount,
                OccurredAt = DateTime.UtcNow,
                AuctionId = auction.Id
            }, ct);

            await walletRepository.AddMovementAsync(new LedgerTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = sellerWallet.Id,
                Type = LedgerTransactionType.Collection,
                Amount = winningBid.Amount,
                OccurredAt = DateTime.UtcNow,
                AuctionId = auction.Id
            }, ct);

            auction.Status = AuctionStatus.Finished;
        }

        private static async Task LogClosureAuditAsync(Auction auction, IAuditLogRepository auditLogRepository, CancellationToken ct)
        {
            await auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = AuditEntityType.Auction,
                EntityId = auction.Id.ToString(),
                Action = auction.Status == AuctionStatus.Finished ? "CierreConGanador" : "CierreDesierta",
                UserId = null,
                DetailsJson = "{\"origen\":\"AuctionClosingWorker\"}",
                OccurredAt = DateTime.UtcNow
            }, ct);
        }
    }
}
