namespace ExaltedCharm.Api.Models
{
    public class PagingMetaData
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}