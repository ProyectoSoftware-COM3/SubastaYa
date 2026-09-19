using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SubastaYaDbContext _context;
        public CategoryRepository(SubastaYaDbContext context) => _context = context;

        public Task<List<Category>> GetAllAsync(CancellationToken ct = default)
            => _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
    }
}

