using AutoMapper;
using Ecommerce_Final.Controllers;
using Ecommerce_Final.Model;

namespace Ecommerce_Final.Common
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            // Mapping from Cart entity to CartDto
            CreateMap<Cart, CartDto>();

            // Mapping from CartItem entity to CartItemDto
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name)); // Map Product Name
            CreateMap<CartItem, CartItemDto>()
                 .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));
        }
    }
}
