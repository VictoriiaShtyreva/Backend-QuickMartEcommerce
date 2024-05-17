
using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities.CartAggregate
{
    public class CartItem : TimeStamp
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual Product? Product { get; set; }

        public CartItem() { }
        public CartItem(Guid cartId, Guid productId, int quantity)
        {
            Guard.Against.Default(productId, nameof(productId));
            Guard.Against.Default(cartId, nameof(cartId));
            Guard.Against.Negative(quantity, nameof(quantity));

            Id = Guid.NewGuid();
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to add quantity to the cart item
        public void AddQuantity(int quantityToAdd)
        {
            Guard.Against.NegativeOrZero(quantityToAdd, nameof(quantityToAdd));
            Quantity += quantityToAdd;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to reduce quantity of the cart item
        public void ReduceQuantity(int quantityToReduce)
        {
            Guard.Against.Negative(quantityToReduce, nameof(quantityToReduce));
            Quantity -= quantityToReduce;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}