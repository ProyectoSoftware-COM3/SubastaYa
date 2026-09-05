using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Infrastructure.Persistence.Configurations
{
    public class LedgerTransactionConfiguration : IEntityTypeConfiguration<LedgerTransaction>
    {
        public void Configure(EntityTypeBuilder<LedgerTransaction> builder)
        {
            builder.ToTable("LedgerTransactions");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Amount).HasPrecision(18, 2);

            builder.HasOne(l => l.Wallet).WithMany(w => w.Movements).HasForeignKey(l => l.WalletId);
          
            builder.HasIndex(l => l.AuctionId);
        }
    }
}
