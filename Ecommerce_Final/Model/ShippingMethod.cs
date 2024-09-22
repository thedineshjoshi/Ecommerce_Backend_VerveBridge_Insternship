namespace Ecommerce_Final.Model
{
    public class ShippingMethod
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int DeliveryTimeInDays { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
