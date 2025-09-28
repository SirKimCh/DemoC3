using BanhMyIT.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanhMyIT.Interface
{
    public interface IBillService
    {
        Task<IEnumerable<Bill>> GetAllAsync();
        Task<Bill> GetByIdAsync(int id);
        Task AddAsync(Bill bill);
        Task UpdateAsync(Bill bill);
        Task DeleteAsync(int id);
    }
}

