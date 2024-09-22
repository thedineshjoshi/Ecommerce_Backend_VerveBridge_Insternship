using Ecommerce_Final.Data;
using Ecommerce_Final.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ecommerce_Final.Controllers
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        private string GeneratePaymentReference()
        {
            var random = new Random();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomNumber = random.Next(1000, 9999);
            return $"PAY-{timestamp}-{randomNumber}";
        }
        public async Task<Order> CreateOrder(Guid userId, string shippingAddress, string billingAddress, string paymentMethod)
        {
            var cart = await _context.Carts
                            .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                            .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
            {
                Console.WriteLine("Cart not found for user.");
                throw new Exception("Cart is empty.");
            }
            if (!cart.CartItems.Any())
            {
                Console.WriteLine("Cart is empty.");
                throw new Exception("Cart is empty.");
            }

            foreach (var item in cart.CartItems)
            {
                Console.WriteLine($"Product: {item.Product.Name}, Price: {item.Product.Price}, Quantity: {item.Quantity}");
            }
            var orderItems = cart.CartItems.Select(ci =>
            {
                var discountedPrice = ci.Product.Price - (ci.Product.Price * ci.Product.Discount / 100);
                return new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = discountedPrice, // Use discounted price
                    DiscountApplied = ci.Product.Discount,
                };
            }).ToList();
            var totalAmount = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
            var random = new Random();
            
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = totalAmount,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Pending",
                PaymentReference = GeneratePaymentReference(),
                ShippingAddress = shippingAddress,
                BillingAddress = billingAddress,
                TrackingNumber = $"TRK-{DateTime.UtcNow:yyyyMMddHHmmss}-{random.Next(1000, 9999)}",
                ShippedDate = DateTime.Now,
                DeliveredDate= DateTime.Now,
                UserId = userId,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price,
                    DiscountApplied = ci.Product.Discount,
                }).ToList()
            };
           // var totalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            Console.WriteLine($"Total Amount calculated: {totalAmount}");
            _context.Orders.Add(order);
            cart.IsActive = false; // Mark cart as inactive
            await _context.SaveChangesAsync();
            _context.Carts.Update(cart);

            return order;
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
