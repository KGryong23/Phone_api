namespace Phone_api.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; }
        public int TotalRecords { get; }

        public PagedResult(IEnumerable<T> data, int totalRecords)
        {
            Data = data;
            TotalRecords = totalRecords;
        }
    }
}
