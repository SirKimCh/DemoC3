using BanhMyIT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanhMyIT.Interface
{
    public interface IBillService
    {
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<IEnumerable<Bill>> GetByUserAsync(int userId);
        Task<Bill?> GetByIdAsync(int id);
        Task AddAsync(Bill bill);
        Task UpdateAsync(Bill bill);
        Task DeleteAsync(int id);
        Task<Bill> CreateFromCartAsync(int cartId, PayMethod? payMethod);
        Task RecalculateTotalAsync(int billId);

        // New: query with search/sort/pagination for admin/staff
        Task<(IEnumerable<Bill> Items, int TotalCount)> QueryAsync(
            string? search, OrderStatus? status, string? sortBy, bool desc, int page, int pageSize);

        // New: status transitions
        Task<bool> ConfirmAsync(int billId);
        Task<bool> MarkShippedAsync(int billId);
        Task<bool> MarkCompletedByUserAsync(int billId, int userId);
        Task<bool> RequestCancelAsync(int billId, int userId, string? reason);
        Task<bool> AdminCancelAsync(int billId, string? reason);
        Task<bool> ApproveCancelAsync(int billId, string? reason);
        Task<bool> RejectCancelAsync(int billId);
    }
}
