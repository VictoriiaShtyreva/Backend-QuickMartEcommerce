using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.WebAPI.src.ExternalService;

namespace Ecommerce.WebAPI.src.Data
{
    public class SeedingData
    {
        private static Random random = new Random();
        private static PasswordService? _passwordService;

        public SeedingData(PasswordService passwordService)
        {
            _passwordService = passwordService;
        }
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

            for (int i = 1; i <= count; i++)
            {
                var product = new Product($"{category.Name} Product {i}", GetRandomNumber() * 100, $"Description for {category.Name} Product {i}", category.Id, 100);
                products.Add(product);
            }
            return products;
        }
        public static List<Product> GetProducts()
        {
            var products = new List<Product>();

            foreach (var category in GetCategories())
            {
                products.AddRange(GenerateProductsForCategory(category, 10));
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
                new User("Alice", "alice@example.com", "", $"https://picsum.photos/200/?random={GetRandomNumberForImage()}", UserRole.Admin, "123 Anywhere St", "Apt 2", 12345, "Anytown", "USA"),
                new User("Bob", "bob@example.com", "", $"https://picsum.photos/200/?random={GetRandomNumberForImage()}", UserRole.Customer, "456 Somewhere Ave", "Suite 300", 54321, "Othertown", "USA"),
                new User("Carol", "carol@example.com", "", $"https://picsum.photos/200/?random={GetRandomNumberForImage()}", UserRole.Customer, "789 Nowhere Blvd", "Box 5", 98765, "Lostcity", "USA")
            };

            // Hash passwords 
            foreach (var user in users)
            {
                user.Password = _passwordService!.HashPassword(user, "Password@123");
            }

            return users;
        }
        #endregion
    }
}