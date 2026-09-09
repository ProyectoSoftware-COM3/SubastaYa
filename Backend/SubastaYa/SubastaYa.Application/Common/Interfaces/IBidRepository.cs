using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Common.Interfaces
{
    public interface IBidRepository
    {
        Task<List<Bid>> GetByBidderIdAsync(Guid bidderId, CancellationToken ct = default);
    }
}
