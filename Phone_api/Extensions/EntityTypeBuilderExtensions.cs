using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phone_api.Entities;

namespace Phone_api.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Cấu hình chung cho các thuộc tính của BaseDomainEntity
        /// </summary>
        public static void ConfigureBaseDomainEntity<T>(this EntityTypeBuilder<T> builder)
            where T : BaseDomainEntity
        {
            // Cấu hình khóa chính (Id)
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Id)
                .IncludeProperties(p => new { p.Created, p.ModerationStatus });

            builder.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");

            // Cấu hình Created
            builder.Property(e => e.Created)
                .IsRequired();

            // Cấu hình LastModified
            builder.Property(e => e.LastModified)
                .IsRequired();

            // Cấu hình ModerationStatus
            builder.Property(e => e.ModerationStatus)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
