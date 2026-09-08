using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Common.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<LedgerTransaction>> GetMovementsAsync(Guid walletId, CancellationToken ct = default);
    }
}
