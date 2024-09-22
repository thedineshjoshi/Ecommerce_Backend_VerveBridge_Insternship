namespace Ecommerce_Final.Model
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; }
        public double Weight { get; set; }
        public string Dimensions { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public decimal Discount { get; set; }
        public bool IsFeatured { get; set; }
    }
}
