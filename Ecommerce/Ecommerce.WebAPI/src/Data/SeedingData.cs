using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.WebAPI.src.Data
{
    public class SeedingData
    {
        private static Random random = new Random();
        private static int GetRandomNumber()
        {
            return random.Next(1, 11);
        }
        private static int GetRandomNumberForImage()
        {
            return random.Next(100, 1000);
        }

        #region Categories
        private static List<Category> categories = new List<Category>
        {
            new Category("Men", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
            new Category("Women", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
            new Category("Electronics", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
            new Category("Jewelry", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
            new Category("Books", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
            new Category("Toys", $"https://picsum.photos/200/?random={GetRandomNumber()}")
        };

        public static List<Category> GetCategories() => categories;
        #endregion

        #region Products
        public static List<Product> GenerateProductsForCategory(Category category, int count)
        {
            var products = new List<Product>();
            var materials = new List<string> { "wood", "metal", "plastic", "glass", "composite" };
            var features = new List<string> { "durable", "lightweight", "eco-friendly", "compact", "ergonomic design" };
            var uses = new List<string> { "indoor", "outdoor", "personal", "commercial", "educational" };

            for (int i = 1; i <= count; i++)
            {
                var material = materials[random.Next(materials.Count)];
                var feature = features[random.Next(features.Count)];
                var use = uses[random.Next(uses.Count)];
                var description = $"The {category.Name} Product {i} is a {feature}, {material}-made product suitable for {use} use. With its {GetRandomNumber()}% satisfaction rating, it's perfect for any {category.Name!.ToLower()} needs.";
                var product = new Product($"{category.Name} Product {i}", GetRandomNumber() * 100, description, category.Id, 100);
                products.Add(product);
            }
            return products;
        }
        public static List<Product> GetProducts()
        {
            var products = new List<Product>();

            foreach (var category in GetCategories())
            {
                products.AddRange(GenerateProductsForCategory(category, 20));
            }
            return products;
        }
        #endregion

        #region Product Images
        public static List<ProductImage> GenerateProductImagesForProduct(Guid productId)
        {
            var productImages = new List<ProductImage>();
            for (int i = 0; i < 3; i++)
            {
                var productImage = new ProductImage(productId, $"https://picsum.photos/200/?random={GetRandomNumberForImage()}");
                productImages.Add(productImage);
            }
            return productImages;
        }

        public static List<ProductImage> GetAllProductImages()
        {
            var allProductImages = new List<ProductImage>();
            var allProducts = GetProducts();

            foreach (var product in allProducts)
            {
                allProductImages.AddRange(GenerateProductImagesForProduct(product.Id));
            }
            return allProductImages;
        }
        #endregion

        #region Users
        public static List<User> GetUsers()
        {
            var users = new List<User>
            {
               new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Alice",
                    Email = "alice@example.com",
                    Password = "alice@123",
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
                    Role = UserRole.Admin,
                    AddressLine1 = "123 Anywhere St",
                    AddressLine2 = "Apt 2",
                    PostCode = 12345,
                    City = "Anytown",
                    Country = "USA"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Bob",
                    Email = "bob@example.com",
                    Password = "bob@123",
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
                    Role = UserRole.Customer,
                    AddressLine1 = "456 Somewhere Ave",
                    AddressLine2 = "Suite 300",
                    PostCode = 54321,
                    City = "Othertown",
                    Country = "USA"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Carol",
                    Email = "carol@example.com",
                    Password = "carol@123",
                    Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
                    Role = UserRole.Customer,
                    AddressLine1 = "789 Nowhere Blvd",
                    AddressLine2 = "Box 5",
                    PostCode = 98765,
                    City = "Lostcity",
                    Country = "USA"
                }
            };
            return users;
        }
        #endregion
    }
}