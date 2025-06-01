using Phone_api.Data;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    /// <summary>
    /// Repository cho Brand, kế thừa Generic Repository
    /// </summary>
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(PhoneContext context) : base(context)
        {
        }
    }
}
