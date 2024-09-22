using System.ComponentModel.DataAnnotations;

namespace Ecommerce_Final.Model
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; } 
        public byte[] RowVersion { get; set; }  // Concurrency token
        public ICollection<CartItem> CartItems { get; set; }
    }
}
