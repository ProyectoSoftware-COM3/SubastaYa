using Microsoft.EntityFrameworkCore;
using SubastaYa.Domain.Entities;
using SubastaYa.Infrastructure.Persistence.Converters;

namespace SubastaYa.Infrastructure.Persistence
{
    public class SubastaYaDbContext : DbContext
    {
        public SubastaYaDbContext(DbContextOptions<SubastaYaDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Auction> Auctions => Set<Auction>();
        public DbSet<Domain.Entities.Wallet> Wallets => Set<Domain.Entities.Wallet>();
        public DbSet<Bid> Bids => Set<Bid>();
        public DbSet<LedgerTransaction> LedgerTransactions => Set<LedgerTransaction>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubastaYaDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        }
    }
}
