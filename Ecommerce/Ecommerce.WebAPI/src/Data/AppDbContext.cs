using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.WebAPI.src.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;

        #region Properties
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSnapshot> ProductSnapshots { get; set; }
        #endregion

        #region Constructors
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
            ChangeTracker.LazyLoadingEnabled = true;
        }

        static AppDbContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        }
        #endregion

        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
        #endregion

        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Enums registration for PostgreSQL
            modelBuilder.HasPostgresEnum<UserRole>();
            modelBuilder.HasPostgresEnum<OrderStatus>();
            base.OnModelCreating(modelBuilder);

            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(u => u.Id).HasName("users_pkey");
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("users_email_key");
                entity.Property(u => u.Name).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
                entity.Property(u => u.AddressLine1).IsRequired().HasMaxLength(255);
                entity.Property(u => u.AddressLine2).HasMaxLength(255);
                entity.Property(u => u.PostCode).IsRequired().HasColumnType("integer");
                entity.Property(u => u.City).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Country).IsRequired().HasMaxLength(100);
            });
            modelBuilder.Entity<User>().ToTable(u => u.HasCheckConstraint("users_avatar_check", "avatar LIKE 'http%' OR avatar = ''"));

            // Order Entity Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(o => o.Id).HasName("orders_pkey");
                entity.Property(o => o.TotalPrice).HasPrecision(18, 2);
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(o => o.UpdatedAt).HasDefaultValueSql("now()");
                entity.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);
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
                entity.Property(p => p.Description).HasColumnType("text");
                entity.Property(p => p.Price).HasColumnType("numeric").HasPrecision(18, 2);
                entity.Property(p => p.Inventory).HasColumnType("integer").HasDefaultValue(0);
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

            // Cart Entity Configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("carts");
                entity.HasKey(c => c.Id).HasName("carts_pkey");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
                entity.HasOne(c => c.User).WithOne(u => u.Cart).HasForeignKey<Cart>(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
                // Configure the navigation property to use a specific backing field
                var navigation = entity.Metadata.FindNavigation(nameof(Cart.CartItems));
                navigation!.SetField("_items");
                navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            });
            modelBuilder.Entity<Cart>().ToTable(c => c.HasCheckConstraint("carts_updated_at_check", "updated_at >= created_at"));

            // CartItem Entity Configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("cart_items");
                entity.HasKey(ci => ci.Id).HasName("cart_items_pkey");
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
                entity.HasOne(ci => ci.Cart).WithMany(c => c.CartItems).HasForeignKey(ci => ci.CartId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ci => ci.Product).WithMany(p => p.CartItems).HasForeignKey(ci => ci.ProductId).OnDelete(DeleteBehavior.SetNull);
            });
            modelBuilder.Entity<CartItem>().ToTable(c => c.HasCheckConstraint("cart_items_updated_at_check", "updated_at >= created_at"));
            modelBuilder.Entity<CartItem>().ToTable(c => c.HasCheckConstraint("cart_items_quantity_check", "quantity > 0"));

            // OrderItem Entity Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(oi => oi.Id).HasName("order_items_pkey");
                entity.Property(oi => oi.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(oi => oi.UpdatedAt).HasDefaultValueSql("now()");
                entity.Property(oi => oi.Price).HasPrecision(18, 2);
                entity.HasOne(oi => oi.Order).WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId).OnDelete(DeleteBehavior.Cascade);
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
                entity.HasOne(r => r.Product).WithMany(p => p.Reviews).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.SetNull);
                entity.Property(oi => oi.CreatedAt).HasDefaultValueSql("now()");
                entity.Property(oi => oi.UpdatedAt).HasDefaultValueSql("now()");
            });
            modelBuilder.Entity<Review>().ToTable(r => r.HasCheckConstraint("reviews_updated_at_check", "updated_at >= created_at"));
            modelBuilder.Entity<Review>().ToTable(r => r.HasCheckConstraint("reviews_rating_check", "rating >= 1 AND rating <= 5"));

            // Fetch seed data
            SeedData(modelBuilder);
        }
        #endregion

        private void SeedData(ModelBuilder modelBuilder)
        {
            var categories = SeedingData.GetCategories();
            modelBuilder.Entity<Category>().HasData(categories);

            var products = SeedingData.GetProducts();
            modelBuilder.Entity<Product>().HasData(products);

            var productImages = new List<ProductImage>();
            foreach (var product in products)
            {
                var imagesForProduct = SeedingData.GenerateProductImagesForProduct(product.Id);
                productImages.AddRange(imagesForProduct);
            }
            modelBuilder.Entity<ProductImage>().HasData(productImages);

            var users = SeedingData.GetUsers();
            modelBuilder.Entity<User>().HasData(users);
        }
    }
}