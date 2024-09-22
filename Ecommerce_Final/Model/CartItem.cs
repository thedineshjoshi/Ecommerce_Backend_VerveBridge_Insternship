using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Final.Model
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }
        public decimal UnitPrice { get; set; }
        public string ImageUrl { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal TotalPrice => Quantity * (UnitPrice - DiscountApplied);
        [Timestamp]
        public byte[] RowVersion { get; set; }  // Concurrency token
    }
}
