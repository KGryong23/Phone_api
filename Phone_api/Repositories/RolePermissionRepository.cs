using Phone_api.Data;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(PhoneContext context) : base(context)
        {
        }
    }
}
