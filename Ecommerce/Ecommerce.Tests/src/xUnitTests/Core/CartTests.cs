using Ecommerce.Core.src.Entities.CartAggregate;

namespace Ecommerce.Tests.src.Core
{
    public class CartTests
    {
        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000001", 1)]
        [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", 2)]
        public void AddItem_ShouldAddNewItem_WhenItemDoesNotExist(Guid cartId, Guid productId, int quantity)
        {
            // Arrange
            var cart = new Cart(cartId);

            // Act
            cart.AddItem(new CartItem(cartId, productId, quantity));

            // Assert
            Assert.Single(cart.CartItems!);
            Assert.Contains(cart.CartItems!, item => item.ProductId == productId && item.Quantity == quantity);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000001", 1, 2)]
        public void AddItem_ShouldUpdateQuantity_WhenItemExists(Guid cartId, Guid productId, int initialQuantity, int additionalQuantity)
        {
            // Arrange
            var cart = new Cart(cartId);
            cart.AddItem(new CartItem(cartId, productId, initialQuantity));

            // Act
            cart.AddItem(new CartItem(cartId, productId, additionalQuantity));

            // Assert
            Assert.Single(cart.CartItems!);
            Assert.Contains(cart.CartItems!, item => item.ProductId == productId && item.Quantity == initialQuantity + additionalQuantity);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000001", 2, 1)]
        public void RemoveItem_ShouldReduceQuantity_WhenQuantityIsLessThanExisting(Guid cartId, Guid productId, int initialQuantity, int reduceQuantity)
        {
            // Arrange
            var cart = new Cart(cartId);
            cart.AddItem(new CartItem(cartId, productId, initialQuantity));

            // Act
            cart.RemoveItem(new CartItem(cartId, productId, reduceQuantity));

            // Assert
            Assert.Single(cart.CartItems!);
            Assert.Contains(cart.CartItems!, item => item.ProductId == productId && item.Quantity == initialQuantity - reduceQuantity);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000001", 1, 1)]
        public void RemoveItem_ShouldRemoveItem_WhenQuantityIsEqualToExisting(Guid cartId, Guid productId, int initialQuantity, int reduceQuantity)
        {
            // Arrange
            var cart = new Cart(cartId);
            cart.AddItem(new CartItem(cartId, productId, initialQuantity));

            // Act
            cart.RemoveItem(new CartItem(cartId, productId, reduceQuantity));

            // Assert
            Assert.Empty(cart.CartItems!);
        }

        [Fact]
        public void ClearCart_ShouldRemoveAllItems()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            cart.AddItem(new CartItem(cart.Id, Guid.NewGuid(), 1));
            cart.AddItem(new CartItem(cart.Id, Guid.NewGuid(), 2));

            // Act
            cart.ClearCart();

            // Assert
            Assert.Empty(cart.CartItems!);
        }
    }
}
