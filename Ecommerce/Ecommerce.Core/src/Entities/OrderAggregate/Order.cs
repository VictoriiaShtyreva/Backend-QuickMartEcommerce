using Ardalis.GuardClauses;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Entities.OrderAggregate
{
    public class Order : TimeStamp
    {
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Processing;
        public decimal TotalPrice { get; set; } = 0;
        private readonly HashSet<OrderItem>? _orderItems;
        public virtual IReadOnlyCollection<OrderItem>? OrderItems => _orderItems;
        public virtual User? User { get; set; }

        public Order() { }
        public Order(Guid userId)
        {
            Guard.Against.Default(userId, nameof(userId));
            Id = Guid.NewGuid();
            UserId = userId;
            _orderItems = new HashSet<OrderItem>(new OrderItemComparer());
        }

        // Method to create an OrderItem and add it to the order
        public void AddOrderItem(Product product, int quantity)
        {

            var productSnapshot = product.CreateSnapshot();
            var existingItem = _orderItems.FirstOrDefault(i => i.ProductSnapshot.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var orderItem = new OrderItem(Id, productSnapshot, quantity);
                _orderItems.Add(orderItem);
            }
            CalculateTotalPrice();
        }

        // Method to calculate the total price of the order
        public void CalculateTotalPrice()
        {
            TotalPrice = _orderItems.Sum(item => item.Price * item.Quantity);
        }

        // A custom equality comparer for OrderItem to define uniqueness in the HashSet
        public class OrderItemComparer : IEqualityComparer<OrderItem>
        {
            public bool Equals(OrderItem? x, OrderItem? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                return x.Id == y.Id;
            }
            public int GetHashCode(OrderItem obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}