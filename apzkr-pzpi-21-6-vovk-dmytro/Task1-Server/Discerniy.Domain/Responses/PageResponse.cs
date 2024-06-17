using Discerniy.Domain.Requests;

namespace Discerniy.Domain.Responses
{
    public class PageResponse<T>
    {
        public long Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public IList<T> Items { get; set; } = new List<T>();

        public PageResponse(IList<T> items, long total, int page, int limit)
        {
            Items = items;
            Total = total;
            Page = page;
            Limit = limit;
        }

        public PageResponse(IList<T> items, long total, PageRequest request)
        {
            Items = items;
            Total = total;
            Page = request.Page;
            Limit = request.Limit;
        }

        public PageResponse<C> Convert<C>(Func<T, C> converter)
        {
            return new PageResponse<C>(Items.Select(converter).ToList(), Total, Page, Limit);
        }
    }
}
