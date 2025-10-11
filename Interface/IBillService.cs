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
    }
}
