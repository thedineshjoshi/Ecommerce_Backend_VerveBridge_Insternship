using Ecommerce_Final.Model;
using Ecommerce_Final.Common;
using Ecommerce_Final.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce_Application_VerveBridge_Internship_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UserRegistrationController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this._context = context;
            _environment = environment;
        }

        // GET: api/UserRegistration
        [HttpGet("Customers")]
        public IActionResult GetCustomers()
        {
            var customers = _context.Users
                .Where(u => u.Role == "Customer")
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    TotalProductsInCart = _context.Carts
                        .Include(c => c.CartItems)
                        .Where(c => c.UserId == u.Id && c.IsActive)
                        .SelectMany(c => c.CartItems)
                        .Count()
                })
                .ToList();

            return Ok(customers);
        }
        [HttpGet("Profile/{id}")]
        public IActionResult GetUserProfile(Guid id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Username,
                    u.Email,
                    u.PhoneNumber,
                    u.ProfileImageUrl,
                    u.Address,
                    u.DateOfBirth,
                    u.IsVerified,
                    u.CreatedAt,
                    u.UpdatedAt,
                    u.Role
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        // GET: api/UserRegistration/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        // POST: api/UserRegistration/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] UserRegistrationDto userRegistration, [FromForm] IFormFile ProfileImageUrl)
        {
            if (_context.Users.Any(u => u.Email == userRegistration.Email || u.Username == userRegistration.Username))
            {
                return BadRequest("Username or Email already exists.");
            }

            // Handle image upload if provided
            if (ProfileImageUrl == null || ProfileImageUrl.Length == 0)
            {
                return BadRequest(new { message = "Image file is required" });
            }

            if (!ProfileImageUrl.ContentType.StartsWith("image/"))
            {
                return BadRequest(new { message = "Uploaded file is not an image." });
            }

            // Define the directory where you want to store the uploaded images
            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, "uploads");

            // Ensure the uploads folder exists
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Save the image file with sanitized name
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImageUrl.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImageUrl.CopyToAsync(fileStream);
            }

            // Create the user object
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                Username = userRegistration.Username,
                Email = userRegistration.Email,
                Role = userRegistration.Role,
                PhoneNumber = userRegistration.PhoneNumber,
                ProfileImageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}",
                Address = userRegistration.Address,
                DateOfBirth = userRegistration.DateOfBirth,
                IsVerified = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PasswordHash = CommonMethods.ConvertToEncrypt(userRegistration.PasswordHash)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/UserRegistration/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, [FromBody] UserRegistrationDto userUpdate)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null) return NotFound("User not found");

            // Update user data
            existingUser.FirstName = userUpdate.FirstName;
            existingUser.LastName = userUpdate.LastName;
            existingUser.Username = userUpdate.Username;
            existingUser.Email = userUpdate.Email;
            existingUser.PhoneNumber = userUpdate.PhoneNumber;
            existingUser.Address = userUpdate.Address;
            existingUser.DateOfBirth = userUpdate.DateOfBirth;
            existingUser.Role = userUpdate.Role;
            existingUser.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return Ok("User updated successfully.");
        }

        // DELETE: api/UserRegistration/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound("User not found");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("User deleted successfully.");
        }
    }
}
