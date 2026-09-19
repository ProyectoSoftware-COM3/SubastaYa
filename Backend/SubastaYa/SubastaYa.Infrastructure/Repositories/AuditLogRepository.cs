using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Persistence;

namespace SubastaYa.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly SubastaYaDbContext _context;
        public AuditLogRepository(SubastaYaDbContext context) => _context = context;

        public async Task AddAsync(AuditLog auditLog, CancellationToken ct = default)
        {
            await _context.AuditLogs.AddAsync(auditLog, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
