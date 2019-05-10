namespace Techies.Client.Stats.Api.Model
{
    public abstract class PagedList
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long TotalItems { get; set; }

        public static PagedList<T> With<T>(T[] items, int page)
        {
            return new PagedList<T>
            {
                Page = page,
                Items = items
            };
        }
    }
    public class PagedList<T>: PagedList
    {
        public T[] Items { get; set; }        

        public PagedList<T> WithTotal(long total)
        {
            TotalItems= total;
            return this;
        }

        public PagedList<T> WithPageSize(int pageSize)
        {
            PageSize = pageSize;
            return this;
        }
    }
}
