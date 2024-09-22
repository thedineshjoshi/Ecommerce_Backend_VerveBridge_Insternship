using Ecommerce_Final.Common;
using Ecommerce_Final.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ecommerce_Final.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<UserWishList> UserWishlists { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
            //Data seeding
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "Developer",
                PasswordHash = CommonMethods.ConvertToEncrypt("@Dineshdj@2080"),
                Role = "Admin",
                Address = "Mnr",  // Provide a value for Address
                Email = "9868459709dinesh@gmail.com",   // Provide other necessary fields if required
                FirstName = "Dinesh",           // Example value for first name
                LastName = "Joshi",             // Example value for last name
                PhoneNumber = "9868504639",    // Example value for phone number
                ProfileImageUrl = "No",
                DateOfBirth = new DateTime(2002, 9, 3),
                IsVerified = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            modelBuilder.Entity<User>().HasData(user);
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); 

           
            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

           
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<UserWishList>()
                .HasKey(uw => new { uw.UserId, uw.ProductId }); 

            modelBuilder.Entity<UserWishList>()
                .HasOne(uw => uw.User)
                .WithMany(u => u.Wishlist)
                .HasForeignKey(uw => uw.UserId);

            modelBuilder.Entity<UserWishList>()
                .HasOne(uw => uw.Product)
                .WithMany(p => p.WishlistedByUsers)
                .HasForeignKey(uw => uw.ProductId);

           
            modelBuilder.Entity<ShippingMethod>()
                .HasMany(sm => sm.Products)
                .WithOne(p => p.ShippingMethod)
                .HasForeignKey(p => p.ShippingMethodId);

            modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Cart>()
       .Property(c => c.TotalPrice)
       .HasColumnType("decimal(18, 2)");

            // CartItem decimal properties
            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.DiscountApplied)
                .HasColumnType("decimal(18, 2)");

            // Order decimal properties
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            // OrderItem decimal properties
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.DiscountApplied)
                .HasColumnType("decimal(18, 2)");

            // Product decimal properties
            modelBuilder.Entity<Product>()
                .Property(p => p.Discount)
                .HasColumnType("decimal(18, 2)");

            // ShippingMethod decimal properties
            modelBuilder.Entity<ShippingMethod>()
                .Property(sm => sm.Cost)
                .HasColumnType("decimal(18, 2)");
            modelBuilder.Entity<Cart>()
                .Property(c => c.RowVersion)
                .IsRowVersion();
        }
    }
}
