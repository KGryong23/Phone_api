using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phone_api.Entities;
using Phone_api.Extensions;

namespace Phone_api.Configurations
{
    /// <summary>
    /// Cấu hình ánh xạ cho thực thể User trong Entity Framework.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Cấu hình bảng và các thuộc tính của User.
        /// </summary>
        /// <param name="builder">Builder để cấu hình ánh xạ.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Cấu hình bảng
            builder.ToTable("Users");

            // Cấu hình từ BaseDomainEntity
            builder.ConfigureBaseDomainEntity();

            // Cấu hình các thuộc tính
            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            // Cấu hình quan hệ một-nhiều với Role
            builder.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
