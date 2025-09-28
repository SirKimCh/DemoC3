using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanhMyIT.Service
{
    public class BillService : IBillService
    {
        private readonly BanhMyITDbContext _context;
        public BillService(BanhMyITDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills.Include(b => b.User).Include(b => b.Product).ToListAsync();
        }
        public async Task<Bill> GetByIdAsync(int id)
        {
            return await _context.Bills.Include(b => b.User).Include(b => b.Product).FirstOrDefaultAsync(b => b.BillID == id);
        }
        public async Task AddAsync(Bill bill)
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return;
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();
        }
    }
}

