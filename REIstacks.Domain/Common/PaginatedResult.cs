// In REIstacks.Domain/Common/PaginatedResult.cs
namespace REIstacks.Domain.Common
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; private set; }
        public int TotalCount { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PaginatedResult(List<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }
}