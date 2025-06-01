using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phone_api.Entities;
using Phone_api.Extensions;

namespace Phone_api.Configurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            // Cấu hình bảng
            builder.ToTable("Brands");

            // Cấu hình từ BaseDomainEntity
            builder.ConfigureBaseDomainEntity();

            // Cấu hình thuộc tính
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
