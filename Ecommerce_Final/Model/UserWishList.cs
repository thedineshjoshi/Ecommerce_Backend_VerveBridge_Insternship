﻿namespace Ecommerce_Final.Model
{
    public class UserWishList
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
