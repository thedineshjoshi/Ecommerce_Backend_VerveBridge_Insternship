﻿namespace Ecommerce_Final.Model
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int TotalProductsInCart { get; set; }
    }
}
