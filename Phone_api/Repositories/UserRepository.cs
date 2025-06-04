using Phone_api.Data;
using Phone_api.Entities;

namespace Phone_api.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(PhoneContext context) : base(context)
        {
        }
    }
}
