using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly SubastaYaDbContext _context;
        public WalletRepository(SubastaYaDbContext context) => _context = context;

        public Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
            => _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);
    }
}
