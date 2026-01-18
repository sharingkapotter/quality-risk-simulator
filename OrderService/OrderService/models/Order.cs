namespace OrderService.models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string User { get; set; } = "";
        public string Product { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
