namespace Phone_api.Entities
{
    public class Phone : BaseDomainEntity
    {
        public string Model { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }
    }
}
