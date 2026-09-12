using Microsoft.AspNetCore.SignalR;
using SubastaYa.Application.Common.Interfaces;

namespace SubastaYa.Infrastructure.RealTime
{
    public class AuctionNotifier : IAuctionNotifier
    {
        private readonly IHubContext<AuctionHub> _hubContext;
        public AuctionNotifier(IHubContext<AuctionHub> hubContext) => _hubContext = hubContext;

        public Task NotifyNewBidAsync(Guid auctionId, decimal currentPrice, string bidderAlias, CancellationToken ct = default)
            => _hubContext.Clients.Group(auctionId.ToString())
                .SendAsync("NewBid", new { auctionId, currentPrice, bidderAlias }, ct);

        public Task NotifyTimeExtendedAsync(Guid auctionId, DateTime newEndDate, CancellationToken ct = default)
            => _hubContext.Clients.Group(auctionId.ToString())
                .SendAsync("TimeExtended", new { auctionId, newEndDate }, ct);

        public Task NotifyAuctionClosedAsync(Guid auctionId, bool hadWinner, CancellationToken ct = default)
            => _hubContext.Clients.Group(auctionId.ToString())
                .SendAsync("AuctionClosed", new { auctionId, hadWinner }, ct);
    }
}