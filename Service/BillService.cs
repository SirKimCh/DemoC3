using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
            return await _context.Bills
                .Include(b => b.User)
                .Include(b => b.BillDetails).ThenInclude(d => d.Product)
                .ToListAsync();
        }
        public async Task<Bill?> GetByIdAsync(int id)
        {
            return await _context.Bills
                .Include(b => b.User)
                .Include(b => b.BillDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(b => b.BillID == id);
        }
        public async Task AddAsync(Bill bill)
        {
            // ensure total price from details if provided
            if (bill.BillDetails?.Any() == true)
            {
                bill.TotalPrice = bill.BillDetails.Sum(d => d.SubTotal);
            }
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
        public async Task<Bill> CreateFromCartAsync(int cartId, PayMethod? payMethod)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
            if (cart == null) throw new InvalidOperationException("Cart not found");
            if (cart.CartDetails == null || cart.CartDetails.Count == 0)
                throw new InvalidOperationException("Cart is empty");
            var bill = new Bill
            {
                UserID = cart.UserID,
                PayMethod = payMethod,
                CreatedAt = DateTime.UtcNow
            };
            foreach (var item in cart.CartDetails)
            {
                var detail = new BillDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal
                };
                bill.BillDetails.Add(detail);
            }
            bill.TotalPrice = bill.BillDetails.Sum(d => d.SubTotal);
            using var tx = await _context.Database.BeginTransactionAsync();
            _context.Bills.Add(bill);
            // clear cart
            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return bill;
        }
        public async Task RecalculateTotalAsync(int billId)
        {
            var bill = await _context.Bills.Include(b => b.BillDetails).FirstOrDefaultAsync(b => b.BillID == billId);
            if (bill == null) return;
            bill.TotalPrice = bill.BillDetails.Sum(d => d.SubTotal);
            await _context.SaveChangesAsync();
        }
    }
}
