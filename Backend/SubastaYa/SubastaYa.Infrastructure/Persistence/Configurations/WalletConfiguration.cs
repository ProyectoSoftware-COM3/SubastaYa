using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SubastaYa.Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Domain.Entities.Wallet>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.TotalBalance).HasPrecision(18, 2);
            builder.Property(w => w.HeldBalance).HasPrecision(18, 2);
            builder.Property(w => w.AvailableBalance).HasPrecision(18, 2);

           
            builder.Property(w => w.Version).IsRowVersion();
        }
    }
}
