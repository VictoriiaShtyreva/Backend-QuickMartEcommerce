using AutoMapper;
using Ecommerce.Core.src.Entities;
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
            CreateMap<User, UserReviewReadDto>();
            // Ignore password when creating a user -> hashing
            CreateMap<UserCreateDto, User>().ForMember(user => user.Password, opt => opt.Ignore());
            // Only map non-null fields to allow partial updates
            CreateMap<UserUpdateDto, User>().ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));
            CreateMap<UserRoleUpdateDto, User>();

            // Category mappings
            CreateMap<Category, CategoryReadDto>();
            CreateMap<CategoryCreateDto, Category>();
            // Only map non-null fields to allow partial updates
            CreateMap<CategoryUpdateDto, Category>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Product mappings
            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews));
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
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForPath(dest => dest.ShippingAddress, opt => opt.Ignore());
            CreateMap<OrderUpdateDto, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Address mappings
            CreateMap<Address, AddressReadDto>();
            CreateMap<AddressCreateDto, Address>();
            // Only map non-null fields to allow partial updates
            CreateMap<AddressUpdateDto, Address>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));


            // OrderItem mappings
            CreateMap<OrderItem, OrderItemReadDto>();
            CreateMap<OrderItemCreateDto, OrderItem>();
            CreateMap<OrderItemUpdateDto, OrderItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            // Review mappings
            CreateMap<Review, ReviewReadDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
            CreateMap<ReviewCreateDto, Review>();
            // Only map non-null fields to allow partial updates
            CreateMap<ReviewUpdateDto, Review>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => !IsValueTypeDefault(srcMember)));

            CreateMap<ProductSnapshot, ProductSnapshotDto>(); // Map ProductSnapshot to ProductSnapshotDto
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