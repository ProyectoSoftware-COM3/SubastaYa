using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Application.Common.Filters;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Common.Interfaces
{
    public interface IAuctionRepository
    {
        Task<(List<Auction> Items, int TotalCount)> GetFilteredAsync(AuctionFilter filter, CancellationToken ct = default);

        Task<Auction?> GetByIdWithBidsAsync(Guid id, CancellationToken ct = default);

        Task AddAsync(Auction auction, CancellationToken ct = default);

        Task<List<Auction>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default);

        Task<Auction?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Auction>> GetExpiredActiveAsync(CancellationToken ct = default);


    }
}
