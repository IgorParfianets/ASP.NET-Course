namespace AspNetArticle.MvcApp.Models
{
    public class PageViewModel
    {
        public int PageNumber { get; }
        public int TotalPages { get; }
        public bool HasFirstPage => PageNumber > 2;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasLastPage => PageNumber < TotalPages - 1;

        public PageViewModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
