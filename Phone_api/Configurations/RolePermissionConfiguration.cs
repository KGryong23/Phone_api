using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phone_api.Entities;

namespace Phone_api.Configurations
{
    /// <summary>
    /// Cấu hình ánh xạ cho thực thể RolePermission trong Entity Framework.
    /// </summary>
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        /// <summary>
        /// Cấu hình bảng và các thuộc tính của RolePermission.
        /// </summary>
        /// <param name="builder">Builder để cấu hình ánh xạ.</param>
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // Cấu hình bảng
            builder.ToTable("RolePermissions");

            // Cấu hình khóa chính composite
            builder.HasKey(x => new { x.RoleId, x.PermissionId });

            // Cấu hình quan hệ
            builder.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
