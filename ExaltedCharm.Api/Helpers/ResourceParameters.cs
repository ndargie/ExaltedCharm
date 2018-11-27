namespace ExaltedCharm.Api.Helpers
{
    public class ResourceParameters
    {
        private const int MaxPageSize = 20;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public string SearchCriteria { get; set; }
        public string OrderBy { get; set; }
        public string Fields { get; set; }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}