using AutoMapper;
using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
namespace Ecommerce.Service.src.Shared
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserReadDto>();
            // Ignore password when creating a user -> hashing
            CreateMap<UserCreateDto, User>().ForMember(user => user.Password, opt => opt.Ignore());
            // Only map non-null fields to allow partial updates
            CreateMap<UserUpdateDto, User>().ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));
            CreateMap<UserRoleUpdateDto, User>();

            // Cart mappings
            CreateMap<Cart, CartReadDto>().ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems));
            CreateMap<CartCreateDto, Cart>();
            // Only map non-null fields to allow partial updates
            CreateMap<CartUpdateDto, Cart>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // CartItem mappings
            CreateMap<CartItem, CartItemReadDto>();
            CreateMap<CartItemCreateDto, CartItem>();
            // Only map non-null fields to allow partial updates
            CreateMap<CartItemUpdateDto, CartItem>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Category mappings
            CreateMap<Category, CategoryReadDto>();
            CreateMap<CategoryCreateDto, Category>();
            // Only map non-null fields to allow partial updates
            CreateMap<CategoryUpdateDto, Category>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Product mappings
            CreateMap<Product, ProductReadDto>();
            CreateMap<ProductCreateDto, Product>();
            // Only map non-null fields to allow partial updates
            CreateMap<ProductUpdateDto, Product>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // ProductImage mappings
            CreateMap<ProductImage, ProductImageReadDto>();
            CreateMap<ProductImageCreateDto, ProductImage>();
            // Only map non-null fields to allow partial updates
            CreateMap<ProductImageUpdateDto, ProductImage>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Order mappings
            CreateMap<Order, OrderReadDto>()
             .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderItems!.Sum(i => i.Price * i.Quantity)))
             .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
             .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress));
            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForPath(dest => dest.ShippingAddress, opt => opt.Ignore());
            CreateMap<OrderUpdateDto, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Address mappings
            CreateMap<Address, AddressDto>();
            CreateMap<AddressDto, Address>();

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemReadDto>();
            CreateMap<OrderItemCreateDto, OrderItem>();
            CreateMap<OrderItemUpdateDto, OrderItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Review mappings
            CreateMap<Review, ReviewReadDto>();
            CreateMap<ReviewCreateDto, Review>();
            // Only map non-null fields to allow partial updates
            CreateMap<ReviewUpdateDto, Review>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Snapshot mappings
            CreateMap<ProductSnapshot, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            .ForMember(dest => dest.Inventory, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.CartItems, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore());

            CreateMap<ProductSnapshot, ProductSnapshotDto>(); // Map ProductSnapshot to ProductSnapshotDto
            CreateMap<OrderItem, OrderItemReadDto>(); // Map OrderItem to OrderItemReadDto
            CreateMap<Order, OrderReadDto>().ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)); // Map Order to OrderReadDto and include OrderItems
        }
        private static bool IsValueTypeDefault(object srcMember)
        {
            if (srcMember == null) return true;
            var type = srcMember.GetType();
            return type.IsValueType && Activator.CreateInstance(type)!.Equals(srcMember);
        }
    }
}