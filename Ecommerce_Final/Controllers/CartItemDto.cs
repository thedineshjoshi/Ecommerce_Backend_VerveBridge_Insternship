namespace Ecommerce_Final.Controllers
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } // Optionally include more product info
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountApplied { get; set; }
        public string ImageUrl { get; set; }
    }
}
