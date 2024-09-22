namespace Ecommerce_Final.Model
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }
        public string SKU { get; set; } 
        public double Weight { get; set; }
        public string Dimensions { get; set; } 
        public string Category { get; set; }
        public string Brand { get; set; }
        public decimal Discount { get; set; } 
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<UserWishList> WishlistedByUsers { get; set; }
        public Guid? ShippingMethodId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
    }
}
