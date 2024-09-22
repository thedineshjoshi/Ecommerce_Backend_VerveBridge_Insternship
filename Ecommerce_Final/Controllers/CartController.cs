using AutoMapper;
using Ecommerce_Final.Controllers;
using Ecommerce_Final.Data;
using Ecommerce_Final.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce_Application_VerveBridge_Internship_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CartController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/<CartController>
        [HttpGet("{userId}")]
        public IActionResult GetCart(Guid userId)
        {
            var cart = _context.Carts.Include(c => c.CartItems)
                                      .ThenInclude(ci => ci.Product)
                                      .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null)
                return NotFound("Cart not found.");

            var cartDto = _mapper.Map<CartDto>(cart);

            return Ok(cartDto);
        }

        // POST api/<CartController>
        [HttpPost("{userId}/cart/{productId}")]
        public async Task<IActionResult> AddProductToCart(Guid userId, Guid productId, [FromQuery] int quantity)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound(new { message = "Product not found." });

            var cart = await _context.Carts.Include(c => c.CartItems)
                                          .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive==true);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.UnitPrice = product.Price;
                cartItem.DiscountApplied = product.Discount;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Product = product,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    DiscountApplied = product.Discount,
                    ImageUrl = product.ImageUrl,
                    CartId = cart.Id
                };
                _context.CartItems.Add(cartItem);
            }

            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.UnitPrice);
            cart.UpdatedAt = DateTime.Now;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.Error.WriteLine($"Concurrency exception: {ex.Message}");
                return Conflict(new { message = "The cart was updated by another user. Please try again." });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"General exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the product to the cart." });
            }

            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(new { message = "Product added to cart successfully.", cart = cartDto });
        }




        // PUT api/<CartController>/
        [HttpPut("{userId}/cart/{productId}")]
        public IActionResult UpdateCartItem(Guid userId, Guid productId, [FromQuery] int quantity)
        {
            var cart = _context.Carts.Include(c => c.CartItems)
                                      .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null)
                return NotFound("Cart not found.");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                return NotFound("Product not in cart.");

            cartItem.Quantity = quantity;
            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * (ci.UnitPrice - (ci.UnitPrice * ci.DiscountApplied / 100)));
            cart.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            var cartDto = _mapper.Map<CartDto>(cart);
            return Ok(cartDto);
        }
        // DELETE api/<CartController>/5
        [HttpDelete("RemoveProduct")]
        public IActionResult RemoveFromCart([FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            var cart = _context.Carts.Include(c => c.CartItems)
                                      .FirstOrDefault(c => c.UserId == userId && (c.IsActive || !c.IsActive));

            if (cart == null)
                return NotFound("Cart not found.");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                return NotFound("Product not in cart.");

            cart.CartItems.Remove(cartItem);
            cart.UpdatedAt = DateTime.Now;
            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * (ci.UnitPrice - (ci.UnitPrice * ci.DiscountApplied / 100)));

            _context.SaveChanges();

            var cartDto = _mapper.Map<CartDto>(cart);

            return Ok(cartDto);
        }
    }
}
