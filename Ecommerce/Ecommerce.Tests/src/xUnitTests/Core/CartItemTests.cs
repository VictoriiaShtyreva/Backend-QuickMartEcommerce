using Ecommerce.Core.src.Entities.CartAggregate;

namespace Ecommerce.Tests.src.Core
{
    public class CartItemTests
    {
        public static IEnumerable<object[]> AddQuantityData()
        {
            yield return new object[] { 5, 3, 8 };
            yield return new object[] { 1, 10, 11 };
            yield return new object[] { 10, 5, 15 };
        }

        public static IEnumerable<object[]> ReduceQuantityData()
        {
            yield return new object[] { 10, 3, 7 };
            yield return new object[] { 5, 5, 0 };
            yield return new object[] { 8, 1, 7 };
        }

        [Theory]
        [MemberData(nameof(AddQuantityData))]
        public void AddQuantity_UpdatesQuantityCorrectly(int initialQuantity, int quantityToAdd, int expectedQuantity)
        {
            // Arrange
            var cartItem = new CartItem(Guid.NewGuid(), Guid.NewGuid(), initialQuantity);

            // Act
            cartItem.AddQuantity(quantityToAdd);

            // Assert
            Assert.Equal(expectedQuantity, cartItem.Quantity);
        }

        [Theory]
        [MemberData(nameof(ReduceQuantityData))]
        public void ReduceQuantity_UpdatesQuantityCorrectly(int initialQuantity, int quantityToReduce, int expectedQuantity)
        {
            // Arrange
            var cartItem = new CartItem(Guid.NewGuid(), Guid.NewGuid(), initialQuantity);

            // Act
            cartItem.ReduceQuantity(quantityToReduce);

            // Assert
            Assert.Equal(expectedQuantity, cartItem.Quantity);
        }
    }
}
