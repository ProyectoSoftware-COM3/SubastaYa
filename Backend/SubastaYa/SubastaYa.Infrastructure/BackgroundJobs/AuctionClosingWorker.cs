using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using SubastaYa.Application.Exceptions;
using SubastaYa.Infrastructure.Persistence;
using SubastaYa.Application.Common.Filters;

namespace SubastaYa.Infrastructure.BackgroundJobs
{
    public class AuctionClosingWorker : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuctionClosingWorker> _logger;
        private const int ScheduledPageSize = 100;

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
                    await ActivateDueScheduledAuctionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error activando subastas programadas");
                }
                
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

        // Solo se buscan los ids: cada subasta se cierra en su propio scope (DbContext y transaccion nuevos),
        // asi si el cierre de una falla, sus cambios pendientes no se guardan junto con la siguiente.
        private async Task CloseExpiredAuctionsAsync(CancellationToken ct)
        {
            List<Guid> expiredAuctionIds;

            using (var scope = _scopeFactory.CreateScope())
            {
                var auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
                var expiredAuctions = await auctionRepository.GetExpiredActiveAsync(ct);
                expiredAuctionIds = expiredAuctions.Select(a => a.Id).ToList();
            }

            foreach (var auctionId in expiredAuctionIds)
            {
                await CloseOneAuctionAsync(auctionId, ct);
            }
        }

        private async Task CloseOneAuctionAsync(Guid auctionId, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
            var bidRepository = scope.ServiceProvider.GetRequiredService<IBidRepository>();
            var walletRepository = scope.ServiceProvider.GetRequiredService<IWalletRepository>();
            var auditLogRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
            var auctionNotifier = scope.ServiceProvider.GetRequiredService<IAuctionNotifier>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var auction = await auctionRepository.GetByIdAsync(auctionId, ct);

            // Se vuelve a validar con la subasta recien leida: una puja de ultimo minuto pudo extender el cierre.
            if (auction is null || auction.Status != AuctionStatus.Active || auction.EndDate > DateTime.UtcNow)
                return;

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                var winningBid = await bidRepository.GetHighestBidAsync(auction.Id, ct);

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
                _logger.LogError(ex, "No se pudo cerrar la subasta {AuctionId}, se reintentara en el proximo ciclo", auctionId);
            }
        }

        

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
        

        private async Task ActivateDueScheduledAuctionsAsync(CancellationToken ct)
        {
            var dueAuctionIds = await GetDueScheduledAuctionIdsAsync(ct);

            foreach (var auctionId in dueAuctionIds)
            {
                await ActivateOneAuctionAsync(auctionId, ct);
            }
        }

        
        private async Task<List<Guid>> GetDueScheduledAuctionIdsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();

            var now = DateTime.UtcNow;
            var dueIds = new List<Guid>();
            var page = 1;

            while (true)
            {
                var filter = new AuctionFilter(AuctionStatus.Scheduled, null, null, null, AuctionSortOrder.LeastTimeRemaining, page, ScheduledPageSize);
                var (items, _) = await auctionRepository.GetFilteredAsync(filter, ct);

                dueIds.AddRange(items.Where(a => a.StartDate <= now).Select(a => a.Id));

                if (items.Count < ScheduledPageSize) break;
                page++;
            }

            return dueIds;
        }

       
        private async Task ActivateOneAuctionAsync(Guid auctionId, CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
            var auditLogRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var now = DateTime.UtcNow;
            var auction = await auctionRepository.GetByIdAsync(auctionId, ct);

            
            if (auction is null || auction.Status != AuctionStatus.Scheduled || auction.StartDate > now)
                return;

            await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                
                auction.Status = auction.EndDate <= now ? AuctionStatus.Unsold : AuctionStatus.Active;

                await LogActivationAuditAsync(auction, auditLogRepository, ct);

                await unitOfWork.SaveChangesAsync(ct);
                await unitOfWork.CommitAsync(ct);
            }
            catch (ConcurrencyConflictException)
            {
                await unitOfWork.RollbackAsync(ct);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);
                _logger.LogError(ex, "No se pudo activar la subasta {AuctionId}, se reintentara en el proximo ciclo", auctionId);
            }
        }

        private static async Task LogActivationAuditAsync(Auction auction, IAuditLogRepository auditLogRepository, CancellationToken ct)
        {
            await auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = AuditEntityType.Auction,
                EntityId = auction.Id.ToString(),
                Action = auction.Status == AuctionStatus.Active ? "ActivacionAutomatica" : "CierreDesierta",
                UserId = null,
                DetailsJson = $"{{\"origen\":\"AuctionClosingWorker\",\"estadoAnterior\":\"Scheduled\",\"estadoNuevo\":\"{auction.Status}\"}}",
                OccurredAt = DateTime.UtcNow
            }, ct);
        }

        
    }
}
