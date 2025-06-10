using Phone_api.Data;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(PhoneContext context) : base(context)
        {
        }
    }
}
