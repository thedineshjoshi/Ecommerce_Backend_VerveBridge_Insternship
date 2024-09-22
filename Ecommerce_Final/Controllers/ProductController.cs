using Ecommerce_Final.Data;
using Ecommerce_Final.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ecommerce_Application_VerveBridge_Internship_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(Guid id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        [HttpGet("recent")]
        public ActionResult<IEnumerable<Product>> GetRecentProducts()
        {
            var recentProducts = _context.Products
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return Ok(recentProducts);
        }
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
        {
            var products = await _context.Products
                .Where(p => p.Category.ToLower() == category.ToLower()) // Use ToLower() for case-insensitive comparison
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return NotFound("No products found for this category.");
            }

            return Ok(products);
        }
        // POST api/<ProductController>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto productDto, [FromForm] IFormFile ImageUrl)
        {
            if (ImageUrl == null || ImageUrl.Length == 0)
            {
                return BadRequest(new { message = "Image file is required" });
            }

            if (!ImageUrl.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { message = "Uploaded file is not an image." });
            }

            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUrl.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ImageUrl.CopyToAsync(fileStream);
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}",
                //ImageUrl = "/uploads/" + uniqueFileName,  // Store relative path to the image
                StockQuantity = productDto.StockQuantity,
                SKU = productDto.SKU,
                Weight = productDto.Weight,
                Dimensions = productDto.Dimensions,
                Category = productDto.Category,
                Brand = productDto.Brand,
                Discount = productDto.Discount,
                IsFeatured = productDto.IsFeatured,
                CreatedAt = DateTime.Now,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
