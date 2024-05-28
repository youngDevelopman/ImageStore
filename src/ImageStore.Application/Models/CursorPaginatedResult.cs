namespace ImageStore.Application.Models
{
    public class CursorPaginatedResult<T>
    {
        public string Start { get; set; }
        public string End { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
