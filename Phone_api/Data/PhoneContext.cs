using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Phone_api.Entities;

namespace Phone_api.Data
{
    public class PhoneContext : DbContext
    {
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public PhoneContext(DbContextOptions<PhoneContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(PhoneContext).Assembly);
            base.OnModelCreating(builder);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseDomainEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.FillDataForInsert();
                        break;
                    case EntityState.Modified:
                        entry.Entity.FillDataForUpdate();
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
