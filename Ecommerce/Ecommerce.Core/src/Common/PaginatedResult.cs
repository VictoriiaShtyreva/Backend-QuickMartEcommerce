namespace Ecommerce.Core.src.Common
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }

        public PaginatedResult(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
    }
}