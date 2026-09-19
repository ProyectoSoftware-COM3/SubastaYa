using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SubastaYaDbContext _context;
        public UserRepository(SubastaYaDbContext context) => _context = context;

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _context.Users.Include(u => u.Wallet).FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
            => await _context.Users.AddAsync(user, ct);
    }
}