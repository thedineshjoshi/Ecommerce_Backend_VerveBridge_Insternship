namespace Ecommerce_Final.Model
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public decimal UnitPrice { get; set; } 
        public decimal DiscountApplied { get; set; } 
        public decimal Subtotal => Quantity * (UnitPrice - DiscountApplied); 
    }

}
