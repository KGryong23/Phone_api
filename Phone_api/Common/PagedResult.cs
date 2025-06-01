namespace Phone_api.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; }
        public int TotalRecords { get; }

        public PagedResult(IEnumerable<T> data, int totalRecords)
        {
            Items = data;
            TotalRecords = totalRecords;
        }
    }
}
