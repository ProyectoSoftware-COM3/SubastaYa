using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Common.Filters;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly SubastaYaDbContext _context;
        public AuctionRepository(SubastaYaDbContext context) => _context = context;

        private static readonly Expression<Func<Auction, decimal>> CurrentPriceSelector =
            a => a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice;

        public async Task<(List<Auction> Items, int TotalCount)> GetFilteredAsync(AuctionFilter filter, CancellationToken ct = default)
        {
            var query = _context.Auctions.Include(a => a.Category).Include(a => a.Bids).AsNoTracking().AsQueryable();

            if (filter.Status is not null) query = query.Where(a => a.Status == filter.Status);
            if (filter.CategoryId is not null) query = query.Where(a => a.CategoryId == filter.CategoryId);
            if (filter.MinPrice is not null)
                query = query.Where(a => (a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice) >= filter.MinPrice);
            if (filter.MaxPrice is not null)
                query = query.Where(a => (a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice) <= filter.MaxPrice);

            query = filter.Sort == AuctionSortOrder.HighestBid
                ? query.OrderByDescending(CurrentPriceSelector)
                : query.OrderBy(a => a.EndDate);

            var totalCount = await query.CountAsync(ct);
            var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);

            return (items, totalCount);
        }

        public Task<Auction?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default)
            => _context.Auctions
                .Include(a => a.Category)
                .Include(a => a.Bids).ThenInclude(b => b.Bidder)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, ct);
    }
}