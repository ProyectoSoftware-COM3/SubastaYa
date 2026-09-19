namespace SubastaYa.Application.Common.Interfaces
{
    public interface IAuctionNotifier
    {
        Task NotifyNewBidAsync(Guid auctionId, decimal currentPrice, string bidderAlias, CancellationToken ct = default);
        Task NotifyTimeExtendedAsync(Guid auctionId, DateTime newEndDate, CancellationToken ct = default);
        Task NotifyAuctionClosedAsync(Guid auctionId, bool hadWinner, CancellationToken ct = default);
    }
}