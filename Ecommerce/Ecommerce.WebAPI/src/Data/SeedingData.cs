// using Ecommerce.Core.src.Entities;
// using Ecommerce.Core.src.Entities.OrderAggregate;
// using Ecommerce.Core.src.ValueObjects;

// namespace Ecommerce.WebAPI.src.Data
// {
//     public class SeedingData
//     {
//         private static Random random = new Random();
//         private static int GetRandomNumber()
//         {
//             return random.Next(1, 11);
//         }
//         private static int GetRandomNumberForImage()
//         {
//             return random.Next(100, 1000);
//         }

//         #region Categories
//         private static List<Category> categories = new List<Category>
//         {
//             new Category("Men", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
//             new Category("Women", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
//             new Category("Electronics", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
//             new Category("Jewelry", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
//             new Category("Books", $"https://picsum.photos/200/?random={GetRandomNumber()}"),
//             new Category("Toys", $"https://picsum.photos/200/?random={GetRandomNumber()}")
//         };

//         public static List<Category> GetCategories() => categories;
//         #endregion

//         #region Products
//         public static List<Product> GenerateProductsForCategory(Category category, int count)
//         {
//             var products = new List<Product>();
//             var materials = new List<string> { "wood", "metal", "plastic", "glass", "composite" };
//             var features = new List<string> { "durable", "lightweight", "eco-friendly", "compact", "ergonomic design" };
//             var uses = new List<string> { "indoor", "outdoor", "personal", "commercial", "educational" };

//             for (int i = 1; i <= count; i++)
//             {
//                 var material = materials[random.Next(materials.Count)];
//                 var feature = features[random.Next(features.Count)];
//                 var use = uses[random.Next(uses.Count)];
//                 var description = $"The {category.Name} Product {i} is a {feature}, {material}-made product suitable for {use} use. With its {GetRandomNumber()}% satisfaction rating, it's perfect for any {category.Name!.ToLower()} needs.";
//                 var product = new Product($"{category.Name} Product {i}", GetRandomNumber() * 100, description, category.Id, 100);
//                 products.Add(product);
//             }
//             return products;
//         }
//         public static List<Product> GetProducts()
//         {
//             var products = new List<Product>();

//             foreach (var category in GetCategories())
//             {
//                 products.AddRange(GenerateProductsForCategory(category, 20));
//             }
//             return products;
//         }
//         #endregion

//         #region Product Images
//         public static List<ProductImage> GenerateProductImagesForProduct(Guid productId)
//         {
//             var productImages = new List<ProductImage>();
//             for (int i = 0; i < 3; i++)
//             {
//                 var productImage = new ProductImage(productId, $"https://picsum.photos/200/?random={GetRandomNumberForImage()}");
//                 productImages.Add(productImage);
//             }
//             return productImages;
//         }

//         public static List<ProductImage> GetAllProductImages()
//         {
//             var allProductImages = new List<ProductImage>();
//             var allProducts = GetProducts();

//             foreach (var product in allProducts)
//             {
//                 allProductImages.AddRange(GenerateProductImagesForProduct(product.Id));
//             }
//             return allProductImages;
//         }
//         #endregion

//         #region Users
//         public static List<User> GetUsers()
//         {
//             var users = new List<User>
//             {
//                new User
//                 {
//                     Id = Guid.NewGuid(),
//                     Name = "Alice",
//                     Email = "alice@example.com",
//                     Password = "alice@123",
//                     Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
//                     Role = UserRole.Admin,
//                 },
//                 new User
//                 {
//                     Id = Guid.NewGuid(),
//                     Name = "Bob",
//                     Email = "bob@example.com",
//                     Password = "bob@123",
//                     Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
//                     Role = UserRole.Customer
//                 },
//                 new User
//                 {
//                     Id = Guid.NewGuid(),
//                     Name = "Carol",
//                     Email = "carol@example.com",
//                     Password = "carol@123",
//                     Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
//                     Role = UserRole.Customer
//                 },
//                 new User
//                 {
//                     Id = Guid.NewGuid(),
//                     Name = "Dave",
//                     Email = "dave@example.com",
//                     Password = "dave@123",
//                     Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
//                     Role = UserRole.Customer
//                 },
//                 new User
//                 {
//                     Id = Guid.NewGuid(),
//                     Name = "Eve",
//                     Email = "eve@example.com",
//                     Password = "eve@123",
//                     Avatar = $"https://picsum.photos/200/?random={GetRandomNumberForImage()}",
//                     Role = UserRole.Customer
//                 }
//             };
//             return users;
//         }
//         #endregion

//         #region Addresses
//         public static List<Address> GetAddresses()
//         {
//             var addresses = new List<Address>
//             {
//                 new Address { Id = Guid.NewGuid(), AddressLine = "123 Main St", City = "Townsville", PostalCode = "12345", Country = "USA" },
//                 new Address { Id = Guid.NewGuid(), AddressLine = "456 Elm St", City = "Villageville", PostalCode = "67890", Country = "USA" },
//                 new Address { Id = Guid.NewGuid(), AddressLine = "789 Oak St", City = "Citytown", PostalCode = "11223", Country = "USA" },
//                 new Address { Id = Guid.NewGuid(), AddressLine = "101 Maple St", City = "Springfield", PostalCode = "33445", Country = "USA" },
//                 new Address { Id = Guid.NewGuid(), AddressLine = "202 Pine St", City = "Greenville", PostalCode = "55667", Country = "USA" },
//                 new Address { Id = Guid.NewGuid(), AddressLine = "303 Cedar St", City = "Hillview", PostalCode = "77889", Country = "USA" }
//             };
//             return addresses;
//         }
//         #endregion

//         #region Orders
//         public static List<Order> GetOrders(List<User> users, List<Address> addresses)
//         {
//             var orders = new List<Order>
//             {
//                 new Order { Id = Guid.NewGuid(), UserId = users[0].Id, AddressId = addresses[0].Id, TotalPrice = 150.50m, Status = OrderStatus.Completed },
//                 new Order { Id = Guid.NewGuid(), UserId = users[1].Id, AddressId = addresses[1].Id, TotalPrice = 250.75m, Status = OrderStatus.Processing },
//                 new Order { Id = Guid.NewGuid(), UserId = users[2].Id, AddressId = addresses[2].Id, TotalPrice = 350.25m, Status = OrderStatus.Shipped },
//                 new Order { Id = Guid.NewGuid(), UserId = users[3].Id, AddressId = addresses[3].Id, TotalPrice = 230.50m, Status = OrderStatus.Completed },
//                 new Order { Id = Guid.NewGuid(), UserId = users[4].Id, AddressId = addresses[4].Id, TotalPrice = 560.99m, Status = OrderStatus.Completed },
//                 new Order { Id = Guid.NewGuid(), UserId = users[1].Id, AddressId = addresses[5].Id, TotalPrice = 450.78m, Status = OrderStatus.Completed }
//             };
//             return orders;
//         }
//         #endregion


//         #region OrderItems
//         public static List<OrderItem> GetOrderItems(List<Order> orders, List<Product> products, List<ProductImage> productImages)
//         {
//             var random = new Random();
//             var orderItems = new List<OrderItem>();
//             foreach (var order in orders)
//             {
//                 var product = products[random.Next(products.Count)];
//                 var images = productImages.Where(img => img.ProductId == product.Id).Select(img => img.Url).ToList();
//                 var snapshot = new ProductSnapshot
//                 {
//                     ProductId = product.Id,
//                     Title = product.Title,
//                     Price = product.Price,
//                     Description = product.Description,
//                     ImageUrls = images!
//                 };
//                 var orderItem = new OrderItem
//                 {
//                     Id = Guid.NewGuid(),
//                     OrderId = order.Id,
//                     ProductSnapshot = snapshot,
//                     Price = snapshot.Price,
//                     Quantity = random.Next(1, 5),
//                     CreatedAt = DateTime.UtcNow,
//                     UpdatedAt = DateTime.UtcNow
//                 };
//                 orderItems.Add(orderItem);
//             }
//             return orderItems;
//         }
//         #endregion

//         #region Reviews
//         public static List<Review> GetReviews(List<User> users, List<Product> products)
//         {
//             var reviews = new List<Review>
//             {
//                 new Review { Id = Guid.NewGuid(), UserId = users[0].Id, ProductId = products[0].Id, Rating = 5, Content = "Excellent product!" },
//                 new Review { Id = Guid.NewGuid(), UserId = users[1].Id, ProductId = products[1].Id, Rating = 4, Content = "Very good quality." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[2].Id, ProductId = products[2].Id, Rating = 3, Content = "Average product." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[1].Id, ProductId = products[3].Id, Rating = 4, Content = "Solid build, satisfied with the purchase." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[2].Id, ProductId = products[4].Id, Rating = 2, Content = "Not as described, disappointed." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[3].Id, ProductId = products[5].Id, Rating = 5, Content = "Outstanding! Exceeded my expectations." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[1].Id, ProductId = products[6].Id, Rating = 3, Content = "It's okay, does the job." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[2].Id, ProductId = products[7].Id, Rating = 4, Content = "Good product, but a bit pricey." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[3].Id, ProductId = products[8].Id, Rating = 5, Content = "Love it! Highly recommend." },
//                 new Review { Id = Guid.NewGuid(), UserId = users[0].Id, ProductId = products[9].Id, Rating = 4, Content = "Works well, as expected." }
//             };
//             return reviews;
//         }
//         #endregion

//     }
// }