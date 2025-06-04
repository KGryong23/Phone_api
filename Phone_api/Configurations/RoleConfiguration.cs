using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phone_api.Entities;
using Phone_api.Extensions;

namespace Phone_api.Configurations
{
    /// <summary>
    /// Cấu hình ánh xạ cho thực thể Role trong Entity Framework.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        /// Cấu hình bảng và các thuộc tính của Role.
        /// </summary>
        /// <param name="builder">Builder để cấu hình ánh xạ.</param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Cấu hình bảng
            builder.ToTable("Roles");

            // Cấu hình từ BaseDomainEntity
            builder.ConfigureBaseDomainEntity();

            // Cấu hình các thuộc tính
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired(false) // Mô tả là tùy chọn
                .HasMaxLength(200);

            // Cấu hình quan hệ một-nhiều với User
            builder.HasMany(x => x.Users)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình quan hệ nhiều-nhiều với Permission
            builder.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
