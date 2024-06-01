using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.Interfaces;
using Ecommerce.WebAPI.src.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly IPasswordService _passwordService;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config, IPasswordService passwordService) : base(options)
        {
            _config = config;
            _passwordService = passwordService;
            ChangeTracker.LazyLoadingEnabled = true;
        }

        static AppDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id).HasName("users_pkey");
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("users_email_key");
                entity.Property(u => u.Name).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
            });
            modelBuilder.Entity<User>().ToTable(u => u.HasCheckConstraint("users_avatar_check", "avatar LIKE 'http%' OR avatar = ''"));

            // Address Entity Configuration
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("addresses");
                entity.HasKey(a => a.Id).HasName("addresses_pkey");
                entity.Property(a => a.AddressLine).IsRequired().HasMaxLength(255);
                entity.Property(a => a.City).IsRequired().HasMaxLength(100);
                entity.Property(a => a.PostalCode).IsRequired();
                entity.Property(a => a.Country).IsRequired().HasMaxLength(100);
            });

            // Order Entity Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(o => o.Id).HasName("orders_pkey");
                entity.Property(o => o.CheckoutUrl);
                entity.Property(o => o.StripeSessionId);
                entity.Property(o => o.TotalPrice).HasPrecision(18, 2);
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(o => o.UpdatedAt).HasDefaultValueSql("now()");
                entity.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(o => o.ShippingAddress).WithOne().HasForeignKey<Order>(o => o.AddressId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Order>().ToTable(o => o.HasCheckConstraint("orders_total_price_check", "total_price > 0"));
            modelBuilder.Entity<Order>().ToTable(o => o.HasCheckConstraint("orders_updated_at_check", "updated_at >= created_at"));

            // Category Entity Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(c => c.Id).HasName("categories_pkey");
                entity.Property(c => c.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(c => c.Name).IsUnique().HasDatabaseName("categories_name_key");
                entity.Property(c => c.Image).IsRequired();
            });
            modelBuilder.Entity<Category>().ToTable(c => c.HasCheckConstraint("categories_image_check", "image LIKE 'http%' OR image = ''"));

            // Product Entity Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(p => p.Id).HasName("products_pkey");
                entity.Property(p => p.Title).IsRequired().HasMaxLength(255);
                entity.HasIndex(p => p.Title).IsUnique().HasDatabaseName("title_unique");
                entity.Property(p => p.Price).HasColumnType("numeric").HasPrecision(18, 2);
                entity.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<Product>().ToTable(p => p.HasCheckConstraint("products_price_check", "price > 0"));

            // ProductImage Entity Configuration
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.ToTable("product_images");
                entity.HasKey(pi => pi.Id).HasName("product_images_pkey");
                entity.Property(pi => pi.Url).IsRequired();
                entity.HasOne(pi => pi.Product).WithMany(p => p.Images).HasForeignKey(pi => pi.ProductId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ProductImage>().ToTable(pi => pi.HasCheckConstraint("product_images_url_check", "url LIKE 'http%' OR url = ''"));

            // OrderItem Entity Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(oi => oi.Id).HasName("order_items_pkey");
                entity.Property(oi => oi.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(oi => oi.UpdatedAt).HasDefaultValueSql("now()");
                entity.Property(oi => oi.Price).HasPrecision(18, 2);
                entity.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(oi => oi.ProductSnapshot)
                      .HasConversion(new JsonValueConverter<ProductSnapshot>()!)
                      .HasColumnType("json");
            });
            modelBuilder.Entity<OrderItem>().ToTable(oi => oi.HasCheckConstraint("order_items_updated_at_check", "updated_at >= created_at"));
            modelBuilder.Entity<OrderItem>().ToTable(oi => oi.HasCheckConstraint("order_items_price_check", "price > 0"));
            modelBuilder.Entity<OrderItem>().ToTable(oi => oi.HasCheckConstraint("order_items_quantity_check", "quantity > 0"));

            // Review Entity Configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("reviews");
                entity.HasKey(r => r.Id).HasName("reviews_pkey");
                entity.HasOne(r => r.User).WithMany(u => u.Reviews).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(r => r.Product).WithMany(p => p.Reviews).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(oi => oi.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(oi => oi.UpdatedAt).HasDefaultValueSql("now()");
            });
            modelBuilder.Entity<Review>().ToTable(r => r.HasCheckConstraint("reviews_updated_at_check", "updated_at >= created_at"));
            modelBuilder.Entity<Review>().ToTable(r => r.HasCheckConstraint("reviews_rating_check", "rating >= 1 AND rating <= 5"));

            // Enums registration for PostgreSQL
            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<OrderStatus>();
            // Fetch seed data
            // SeedData(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        // private void SeedData(ModelBuilder modelBuilder)
        // {
        //     var categories = SeedingData.GetCategories();
        //     modelBuilder.Entity<Category>().HasData(categories);

        //     var products = SeedingData.GetProducts();
        //     modelBuilder.Entity<Product>().HasData(products);

        //     var productImages = new List<ProductImage>();
        //     foreach (var product in products)
        //     {
        //         var imagesForProduct = SeedingData.GenerateProductImagesForProduct(product.Id);
        //         productImages.AddRange(imagesForProduct);
        //     }
        //     modelBuilder.Entity<ProductImage>().HasData(productImages);

        //     var users = SeedingData.GetUsers();
        //     foreach (var user in users)
        //     {
        //         user.Password = _passwordService.HashPassword(user, user.Password!);
        //     }
        //     modelBuilder.Entity<User>().HasData(users);

        //     var addresses = SeedingData.GetAddresses();
        //     modelBuilder.Entity<Address>().HasData(addresses);

        //     var orders = SeedingData.GetOrders(users, addresses);
        //     modelBuilder.Entity<Order>().HasData(orders);

        //     var orderItems = SeedingData.GetOrderItems(orders, products, productImages);
        //     modelBuilder.Entity<OrderItem>().HasData(orderItems);

        //     var reviews = SeedingData.GetReviews(users, products);
        //     modelBuilder.Entity<Review>().HasData(reviews);
        // }
    }
}