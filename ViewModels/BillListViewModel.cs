using BanhMyIT.Models;

namespace BanhMyIT.ViewModels
{
    public class BillListViewModel
    {
        public IEnumerable<Bill> Items { get; set; } = Enumerable.Empty<Bill>();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)(TotalCount == 0 ? 1 : TotalCount) / (PageSize <= 0 ? 10 : PageSize));
        public string? Search { get; set; }
        public OrderStatus? Status { get; set; }
        public string? SortBy { get; set; }
        public bool Desc { get; set; }
        public bool IsMyList { get; set; } = false;
    }
}
