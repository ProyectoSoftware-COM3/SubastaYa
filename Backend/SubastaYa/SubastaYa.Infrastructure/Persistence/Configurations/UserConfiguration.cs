using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(160);
            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Domain.Entities.Wallet>(w => w.UserId);
        }
    }
}
