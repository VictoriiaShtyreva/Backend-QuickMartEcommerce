using Ecommerce.Core.src.Entities;
using Ecommerce.Core.src.Entities.CartAggregate;

namespace Ecommerce.Tests.src.Core
{
    public class CartTests
    {
        [Fact]
        public void AddProduct_ShouldAddNewItem_WhenItemDoesNotExist()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            var product = new Product { Id = Guid.NewGuid(), Title = "Test Product" };
            int quantity = 2;

            // Act
            cart.AddProduct(product, quantity);

            // Assert
            Assert.Single(cart.CartItems!);
            var cartItem = cart.CartItems!.First();
            Assert.Equal(product.Id, cartItem.ProductId);
            Assert.Equal(quantity, cartItem.Quantity);
        }

        [Fact]
        public void AddProduct_ShouldUpdateQuantity_WhenItemAlreadyExists()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            var product = new Product { Id = Guid.NewGuid(), Title = "Test Product" };
            cart.AddProduct(product, 2);
            int additionalQuantity = 3;

            // Act
            cart.AddProduct(product, additionalQuantity);

            // Assert
            Assert.Single(cart.CartItems!);
            var cartItem = cart.CartItems!.First();
            Assert.Equal(product.Id, cartItem.ProductId);
            Assert.Equal(5, cartItem.Quantity); // 2 + 3
        }

        [Fact]
        public void AddProduct_ShouldThrowException_WhenProductIsNull()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            Product? product = null;
            int quantity = 2;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cart.AddProduct(product!, quantity));
        }

        [Fact]
        public void AddProduct_ShouldThrowException_WhenQuantityIsZeroOrNegative()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            var product = new Product { Id = Guid.NewGuid(), Title = "Test Product" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => cart.AddProduct(product, 0));
            Assert.Throws<ArgumentException>(() => cart.AddProduct(product, -1));
        }

        [Fact]
        public void RemoveItem_ShouldReduceQuantity_WhenItemExists()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            var product = new Product { Id = Guid.NewGuid(), Title = "Test Product" };
            cart.AddProduct(product, 5);
            var cartItem = cart.CartItems!.First();

            // Act
            cart.RemoveItem(new CartItem(cart.Id, product.Id, 3));

            // Assert
            var updatedCartItem = cart.CartItems!.First();
            Assert.Equal(2, updatedCartItem.Quantity); // 5 - 3
        }

        [Fact]
        public void RemoveItem_ShouldRemoveItem_WhenQuantityBecomesZero()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            var product = new Product { Id = Guid.NewGuid(), Title = "Test Product" };
            cart.AddProduct(product, 5);
            var cartItem = cart.CartItems!.First();

            // Act
            cart.RemoveItem(new CartItem(cart.Id, product.Id, 5));

            // Assert
            Assert.Empty(cart.CartItems!);
        }

        [Fact]
        public void RemoveItem_ShouldThrowException_WhenCartItemIsNull()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            CartItem? cartItem = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => cart.RemoveItem(cartItem!));
        }

        [Fact]
        public void ClearCart_ShouldClearAllItems()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            var product1 = new Product { Id = Guid.NewGuid(), Title = "Product 1" };
            var product2 = new Product { Id = Guid.NewGuid(), Title = "Product 2" };
            cart.AddProduct(product1, 2);
            cart.AddProduct(product2, 3);

            // Act
            cart.ClearCart();

            // Assert
            Assert.Empty(cart.CartItems!);
        }
    }
}
