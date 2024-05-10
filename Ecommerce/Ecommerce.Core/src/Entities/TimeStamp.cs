namespace Ecommerce.Core.src.Entities
{
    public abstract class TimeStamp : BaseEntity
    {
        public virtual DateTime CreatedAt { get; set; }
        public virtual DateTime UpdatedAt { get; set; }
    }
}