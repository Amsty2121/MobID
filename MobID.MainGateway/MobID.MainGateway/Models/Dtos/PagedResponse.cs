namespace MobID.MainGateway.Models.Dtos
{
    public class PagedResponse<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public IList<T> Items { get; set; }

        public PagedResponse(int pageIndex, int pageSize, int total, IList<T> items)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Total = total;
            Items = items;
        }
    }

}
