namespace Phone_api.Dtos
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Permissions { get; set; } 
    }

    public class CreateRoleRequest
    {
        public string Name { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Name { get; set; }
    }
}
