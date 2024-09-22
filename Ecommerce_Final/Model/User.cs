﻿namespace Ecommerce_Final.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Cart>Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<UserWishList> Wishlist { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }

}
