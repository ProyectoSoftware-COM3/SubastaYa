using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(CancellationToken ct = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}

