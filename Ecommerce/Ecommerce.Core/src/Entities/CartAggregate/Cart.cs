using System.Dynamic;
using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities.CartAggregate
{
    public class Cart : TimeStamp
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }
        private readonly HashSet<CartItem>? _items;
        public IReadOnlyCollection<CartItem>? CartItems => _items;

        public Cart() { }
        public Cart(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = Guard.Against.Default(userId, nameof(userId));
            _items = new HashSet<CartItem>(new CartItemComparer());
        }

        // Method for add item to cart
        public void AddItem(Guid productId, int quantity)
        {
            Guard.Against.NegativeOrZero(quantity, nameof(quantity));

            var existingItem = _items?.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.AddQuantity(quantity);
            }
            else
            {
                _items?.Add(new CartItem(this.Id, productId, quantity));
            }
        }

        // Method to remove an item from the cart
        public void RemoveItem(Guid productId, int quantity)
        {
            Guard.Against.Default(productId, nameof(productId));
            Guard.Against.NegativeOrZero(quantity, nameof(quantity));

            var existingItem = _items?.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem == null) return;
            existingItem.ReduceQuantity(quantity);
            if (existingItem.Quantity == 0)
            {
                _items?.Remove(existingItem);
            }
        }

        // Method to clear the cart
        public void ClearCart()
        {
            _items?.Clear();
        }

        // A custom equality comparer for CartItem to define uniqueness in the HashSet
        public class CartItemComparer : IEqualityComparer<CartItem>
        {
            public bool Equals(CartItem? x, CartItem? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(CartItem obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}