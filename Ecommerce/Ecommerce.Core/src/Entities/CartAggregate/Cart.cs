using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities.CartAggregate
{
    public class Cart : TimeStamp
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        private readonly HashSet<CartItem>? _items;
        public virtual ICollection<CartItem>? CartItems => _items;
        public Cart()
        { }
        public Cart(Guid userId)
        {
            Guard.Against.Default(userId, nameof(userId));
            Id = Guid.NewGuid();
            UserId = userId;
            _items = new HashSet<CartItem>(new CartItemComparer());
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method for add item to cart
        public void AddItem(CartItem cartItem)
        {
            Guard.Against.Null(cartItem, nameof(cartItem));
            Guard.Against.NegativeOrZero(cartItem.Quantity, nameof(cartItem.Quantity));

            var existingItem = _items?.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                existingItem.AddQuantity(cartItem.Quantity);
            }
            else
            {
                _items?.Add(cartItem);
            }
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to remove an item from the cart
        public void RemoveItem(CartItem cartItem)
        {
            Guard.Against.Null(cartItem, nameof(cartItem));
            Guard.Against.Default(cartItem.ProductId, nameof(cartItem.ProductId));
            Guard.Against.NegativeOrZero(cartItem.Quantity, nameof(cartItem.Quantity));

            var existingItem = _items?.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem == null) return;
            existingItem.ReduceQuantity(cartItem.Quantity);
            if (existingItem.Quantity == 0)
            {
                _items?.Remove(existingItem);
            }
            UpdatedAt = DateTime.UtcNow; // Update timestamp
        }

        // Method to clear the cart
        public void ClearCart()
        {
            _items?.Clear();
            UpdatedAt = DateTime.UtcNow; // Update timestamp
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