using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.Repositories
{
    public class BidRepository : IBidRepository
    {
        private readonly SubastaYaDbContext _context;
        public BidRepository(SubastaYaDbContext context) => _context = context;

        public Task<List<Bid>> GetByBidderIdAsync(Guid bidderId, CancellationToken ct = default)
            => _context.Bids
                .Include(b => b.Auction).ThenInclude(a => a.Bids)
                .AsNoTrackingWithIdentityResolution()
                .Where(b => b.BidderId == bidderId)
                .ToListAsync(ct);

        public async Task AddAsync(Bid bid, CancellationToken ct = default)
            => await _context.Bids.AddAsync(bid, ct);

        public Task<Bid?> GetHighestBidAsync(Guid auctionId, CancellationToken ct = default)
            => _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync(ct);
    }
}