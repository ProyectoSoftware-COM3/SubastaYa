using Microsoft.AspNetCore.SignalR;

namespace SubastaYa.Infrastructure.RealTime
{
    public class AuctionHub : Hub
    {
        public async Task JoinAuctionRoom(string auctionId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);

        public async Task LeaveAuctionRoom(string auctionId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId);
    }
}