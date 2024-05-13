using Ecommerce.Core.src.Entities.CartAggregate;

namespace Ecommerce.Tests.src.Core
{
    public class CartTests
    {
        public static IEnumerable<object[]> AddItemData()
        {
            yield return new object[] { new CartItem(Guid.NewGuid(), Guid.NewGuid(), 5), 5, 1 };
            yield return new object[] { new CartItem(Guid.NewGuid(), Guid.NewGuid(), 2), 3, 1 };
        }

        public static IEnumerable<object[]> RemoveItemData()
        {
            yield return new object[] { new CartItem(Guid.NewGuid(), Guid.NewGuid(), 5), 3, 2 };
            yield return new object[] { new CartItem(Guid.NewGuid(), Guid.NewGuid(), 10), 5, 5 };
        }

        [Theory]
        [MemberData(nameof(AddItemData))]
        public void AddItem_UpdatesCartCorrectly(CartItem initialItem, int quantityToAdd, int expectedCount)
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            cart.AddItem(initialItem.ProductId, initialItem.Quantity);

            // Act
            cart.AddItem(initialItem.ProductId, quantityToAdd);

            // Assert
            Assert.Equal(expectedCount, cart.CartItems!.Count);
            Assert.Equal(initialItem.Quantity + quantityToAdd, cart.CartItems.First(i => i.ProductId == initialItem.ProductId).Quantity);
        }

        [Theory]
        [MemberData(nameof(RemoveItemData))]
        public void RemoveItem_UpdatesCartCorrectly(CartItem initialItem, int quantityToRemove, int expectedFinalQuantity)
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            cart.AddItem(initialItem.ProductId, initialItem.Quantity);

            // Act
            cart.RemoveItem(initialItem.ProductId, quantityToRemove);

            // Assert
            var item = cart.CartItems!.FirstOrDefault(i => i.ProductId == initialItem.ProductId);
            Assert.NotNull(item);
            Assert.Equal(expectedFinalQuantity, item.Quantity);
        }

        [Fact]
        public void ClearCart_RemovesAllItems()
        {
            // Arrange
            var cart = new Cart(Guid.NewGuid());
            cart.AddItem(Guid.NewGuid(), 5);
            cart.AddItem(Guid.NewGuid(), 10);

            // Act
            cart.ClearCart();

            // Assert
            Assert.Empty(cart.CartItems!);
        }
    }
}
