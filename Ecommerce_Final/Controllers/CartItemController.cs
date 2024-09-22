using Ecommerce_Final.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce_Final.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartItemController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult UpdateCartItem(Guid userId, Guid productId, int quantity)
        {
            var cart = _context.Carts.Include(c => c.CartItems)
                        .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (cart == null)
                return NotFound("Cart not found.");

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
                return NotFound("Product not in cart.");

            cartItem.Quantity = quantity;
            cart.UpdatedAt = DateTime.Now;
            cart.TotalPrice = cart.CartItems.Sum(ci => ci.TotalPrice);

            _context.SaveChanges();

            return Ok(cart);
        }

        // DELETE api/<ValuesController>/5
    }
}
