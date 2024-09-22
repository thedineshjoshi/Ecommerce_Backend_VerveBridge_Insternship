using Ecommerce_Final.Data;
using Ecommerce_Final.Model;
using Microsoft.EntityFrameworkCore;

public class CartService
{
    private readonly ApplicationDbContext _dbContext;

    public CartService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Cart GetCartByUserId(Guid userId)
    {
        return _dbContext.Carts
                         .Where(c => c.UserId == userId && c.IsActive)
                         .Include(c => c.CartItems)
                         .ThenInclude(ci => ci.Product)
                         .FirstOrDefault();
    }

    public void AddProductToCart(Guid userId, Guid productId, int quantity)
    {
        var cart = GetCartByUserId(userId);
        if (cart == null)
        {
            // Create a new cart if the user doesn't have one
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ExpiryDate =DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };
            _dbContext.Carts.Add(cart);
            _dbContext.SaveChanges();  // Save the cart first
        }

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (cartItem != null)
        {
            // Update quantity if the product is already in the cart
            cartItem.Quantity += quantity;
            cartItem.UnitPrice = cartItem.Product.Price;
            cartItem.DiscountApplied = cartItem.Product.Discount;
        }
        else
        {
            // Add new product to the cart
            var product = _dbContext.Products.Find(productId);
            if (product != null)
            {
                cart.CartItems.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    CartId = cart.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountApplied = product.Discount
                });
            }
        }

        cart.TotalPrice = cart.CartItems.Sum(ci => ci.TotalPrice);
        cart.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
    }

    public void RemoveProductFromCart(Guid userId, Guid productId)
    {
        var cart = GetCartByUserId(userId);
        if (cart == null) return;

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (cartItem != null)
        {
            cart.CartItems.Remove(cartItem);
            cart.TotalPrice = cart.CartItems.Sum(ci => ci.TotalPrice);
            _dbContext.SaveChanges();
        }
    }

    public void UpdateCart(Cart cart)
    {
        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is Cart)
                {
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    if (databaseValues == null)
                    {
                        throw new Exception("The cart was deleted by another user.");
                    }

                    // You can either choose to refresh the entity with the new values
                    entry.OriginalValues.SetValues(databaseValues);
                }
                else
                {
                    throw new NotSupportedException("Concurrency conflict detected for an entity of type " + entry.Metadata.Name);
                }
            }

            // Retry the update after handling concurrency
            _dbContext.SaveChanges();
        }
    }
}