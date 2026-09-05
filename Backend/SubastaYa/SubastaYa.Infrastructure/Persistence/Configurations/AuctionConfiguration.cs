using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Infrastructure.Persistence.Configurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.ToTable("Auctions");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.BasePrice).HasPrecision(18, 2);
            builder.Property(a => a.MinIncrement).HasPrecision(18, 2);

           
            builder.Property(a => a.Version).IsRowVersion();

            builder.HasOne(a => a.Seller).WithMany(u => u.AuctionsCreated).HasForeignKey(a => a.SellerId);
            builder.HasOne(a => a.Category).WithMany(c => c.Auctions).HasForeignKey(a => a.CategoryId);
        }
    }
}
