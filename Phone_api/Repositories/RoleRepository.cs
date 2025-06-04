using Phone_api.Data;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(PhoneContext context) : base(context)
        {
        }
    }
}
