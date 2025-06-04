using Phone_api.Data;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(PhoneContext context) : base(context)
        {
        }
    }
}
