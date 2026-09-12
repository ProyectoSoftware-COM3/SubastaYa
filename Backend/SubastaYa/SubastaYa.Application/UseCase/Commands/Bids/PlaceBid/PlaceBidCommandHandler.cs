using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using SubastaYa.Domain.Exceptions;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Commands.Bids.PlaceBid
{
    public class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, BidDto>
    {
        private const int AntiSnipingWindowSeconds = 60;
        private static readonly TimeSpan AntiSnipingExtension = TimeSpan.FromMinutes(2);

        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAuctionNotifier _auctionNotifier;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceBidCommandHandler(
            IAuctionRepository auctionRepository,
            IBidRepository bidRepository,
            IWalletRepository walletRepository,
            IAuditLogRepository auditLogRepository,
            IAuctionNotifier auctionNotifier,
            IUnitOfWork unitOfWork)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
            _walletRepository = walletRepository;
            _auditLogRepository = auditLogRepository;
            _auctionNotifier = auctionNotifier;
            _unitOfWork = unitOfWork;
        }

        public async Task<BidDto> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var auction = await GetAuctionOrThrowAsync(request.AuctionId, cancellationToken);
                var bidderWallet = await GetWalletOrThrowAsync(request.BidderId, cancellationToken);
                var now = DateTime.UtcNow;

                ValidateAuctionIsActive(auction, now);

                var previousHighestBid = await _bidRepository.GetHighestBidAsync(request.AuctionId, cancellationToken);
                ValidateBidAmount(auction, previousHighestBid, request.Amount);
                ValidateSufficientBalance(bidderWallet, request.BidderId, request.Amount);

                HoldNewBidderFunds(bidderWallet, request.Amount);
                await ReleasePreviousHolderIfAnyAsync(previousHighestBid, auction, now, cancellationToken);

                var wasExtended = ApplyAntiSnipingIfNeeded(auction, now);

                var bid = await RegisterBidAsync(auction, request, now, cancellationToken);
                await RegisterHoldMovementAsync(bidderWallet, bid, now, cancellationToken);

                if (wasExtended)
                    await LogExtensionAuditAsync(auction, request.BidderId, now, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                await NotifyBidPlacedAsync(auction, request.Amount, wasExtended, cancellationToken);

                return new BidDto(bid.Id, "vos", bid.Amount, bid.PlacedAt);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                await LogRejectionAuditAsync(request, ex, cancellationToken);
                throw;
            }

        }
        //Lectura y validaciones (2.2 del TP: Activa -> Monto OK -> Saldo OK)

        private async Task<Auction> GetAuctionOrThrowAsync(Guid auctionId, CancellationToken ct)
            => await _auctionRepository.GetByIdAsync(auctionId, ct)
                ?? throw new AuctionNotFoundException(auctionId);

        private async Task<Domain.Entities.Wallet> GetWalletOrThrowAsync(Guid bidderId, CancellationToken ct)
            => await _walletRepository.GetByUserIdAsync(bidderId, ct)
                ?? throw new WalletNotFoundException(bidderId);

        private static void ValidateAuctionIsActive(Auction auction, DateTime now)
        {
            if (auction.Status != AuctionStatus.Active || now < auction.StartDate || now > auction.EndDate)
                throw new AuctionNotActiveException(auction.Id);
        }

        private static void ValidateBidAmount(Auction auction, Bid? previousHighestBid, decimal amount)
        {
            var currentPrice = previousHighestBid?.Amount ?? auction.BasePrice;
            var minValidAmount = currentPrice + auction.MinIncrement;
            if (amount < minValidAmount)
                throw new InvalidBidException(auction.Id, amount, minValidAmount);
        }

        private static void ValidateSufficientBalance(Domain.Entities.Wallet bidderWallet, Guid bidderId, decimal amount)
        {
            if (amount > bidderWallet.AvailableBalance)
                throw new InsufficientBalanceException(bidderId, amount, bidderWallet.AvailableBalance);
        }
        //Escrow (2.1 del TP)

        private static void HoldNewBidderFunds(Domain.Entities.Wallet bidderWallet, decimal amount)
        {
            bidderWallet.HeldBalance += amount;
            bidderWallet.AvailableBalance = bidderWallet.TotalBalance - bidderWallet.HeldBalance;
        }

        private async Task ReleasePreviousHolderIfAnyAsync(Bid? previousHighestBid, Auction auction, DateTime now, CancellationToken ct)
        {
            if (previousHighestBid is null) return;

            var previousBidderWallet = await _walletRepository.GetByUserIdAsync(previousHighestBid.BidderId, ct);
            if (previousBidderWallet is null) return;

            previousBidderWallet.HeldBalance -= previousHighestBid.Amount;
            previousBidderWallet.AvailableBalance = previousBidderWallet.TotalBalance - previousBidderWallet.HeldBalance;

            await _walletRepository.AddMovementAsync(new LedgerTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = previousBidderWallet.Id,
                Type = LedgerTransactionType.Release,
                Amount = previousHighestBid.Amount,
                OccurredAt = now,
                AuctionId = auction.Id
            }, ct);
        }
        //Anti-sniping (2.2 del TP)

        private bool ApplyAntiSnipingIfNeeded(Auction auction, DateTime now)
        {
            var secondsToClose = (auction.EndDate - now).TotalSeconds;
            if (secondsToClose > AntiSnipingWindowSeconds) return false;

            auction.EndDate = auction.EndDate.Add(AntiSnipingExtension);
            return true;
        }
        //Registro de la puja y sus movimientos

        private async Task<Bid> RegisterBidAsync(Auction auction, PlaceBidCommand request, DateTime now, CancellationToken ct)
        {
            var bid = new Bid
            {
                Id = Guid.NewGuid(),
                AuctionId = auction.Id,
                BidderId = request.BidderId,
                Amount = request.Amount,
                PlacedAt = now
            };
            await _bidRepository.AddAsync(bid, ct);
            return bid;
        }

        private async Task RegisterHoldMovementAsync(Domain.Entities.Wallet bidderWallet, Bid bid, DateTime now, CancellationToken ct)
        {
            await _walletRepository.AddMovementAsync(new LedgerTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = bidderWallet.Id,
                Type = LedgerTransactionType.Hold,
                Amount = bid.Amount,
                OccurredAt = now,
                AuctionId = bid.AuctionId
            }, ct);
        }
        //Auditoria (3.4 del TP)

        private async Task LogExtensionAuditAsync(Auction auction, Guid bidderId, DateTime now, CancellationToken ct)
        {
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = AuditEntityType.Auction,
                EntityId = auction.Id.ToString(),
                Action = "ExtensionAntiSniping",
                UserId = bidderId,
                DetailsJson = $"{{\"nuevaFechaFin\":\"{auction.EndDate:o}\"}}",
                OccurredAt = now
            }, ct);
        }

        private async Task LogRejectionAuditAsync(PlaceBidCommand request, Exception ex, CancellationToken ct)
        {
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = AuditEntityType.Auction,
                EntityId = request.AuctionId.ToString(),
                Action = "PujaRechazada",
                UserId = request.BidderId,
                DetailsJson = $"{{\"motivo\":\"{ex.GetType().Name}\"}}",
                OccurredAt = DateTime.UtcNow
            }, ct);
        }
        //Tiempo real (Modulo 3, SignalR)

        private async Task NotifyBidPlacedAsync(Auction auction, decimal amount, bool wasExtended, CancellationToken ct)
        {
            await _auctionNotifier.NotifyNewBidAsync(auction.Id, amount, "vos", ct);
            if (wasExtended)
                await _auctionNotifier.NotifyTimeExtendedAsync(auction.Id, auction.EndDate, ct);
        }

    }
}