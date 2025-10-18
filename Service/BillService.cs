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
                .Where(b => !b.IsDeleted)
                .Include(b => b.User)
                .Include(b => b.BillDetails).ThenInclude(d => d.Product)
                .ToListAsync();
        }
        public async Task<IEnumerable<Bill>> GetByUserAsync(int userId)
        {
            return await _context.Bills
                .Where(b => b.UserID == userId && !b.IsDeleted)
                .Include(b => b.User)
                .Include(b => b.BillDetails).ThenInclude(d => d.Product)
                .ToListAsync();
        }
        public async Task<Bill?> GetByIdAsync(int id)
        {
            return await _context.Bills
                .Include(b => b.User)
                .Include(b => b.BillDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(b => b.BillID == id && !b.IsDeleted);
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
            bill.IsDeleted = true;
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
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending
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

        public async Task<(IEnumerable<Bill> Items, int TotalCount)> QueryAsync(string? search, OrderStatus? status, string? sortBy, bool desc, int page, int pageSize)
        {
            if (page < 1) page = 1; if (pageSize < 1) pageSize = 10; if (pageSize > 200) pageSize = 200;
            IQueryable<Bill> q = _context.Bills.Where(b => !b.IsDeleted);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(b => b.BillID.ToString().Contains(s) ||
                                 (b.User.FirstMidName + " " + b.User.LastName).ToLower().Contains(s));
            }
            if (status.HasValue)
            {
                q = q.Where(b => b.Status == status.Value);
            }
            q = (sortBy, desc) switch
            {
                ("created", true) => q.OrderByDescending(b => b.CreatedAt),
                ("created", false) => q.OrderBy(b => b.CreatedAt),
                ("total", true) => q.OrderByDescending(b => b.TotalPrice),
                ("total", false) => q.OrderBy(b => b.TotalPrice),
                ("status", true) => q.OrderByDescending(b => b.Status),
                ("status", false) => q.OrderBy(b => b.Status),
                _ when desc => q.OrderByDescending(b => b.BillID),
                _ => q.OrderBy(b => b.BillID)
            };
            var total = await q.CountAsync();
            var pageQ = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Include(b => b.User)
                .Include(b => b.BillDetails);
            var items = await pageQ.ToListAsync();
            return (items, total);
        }

        public async Task<bool> ConfirmAsync(int billId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && !b.IsDeleted);
            if (bill == null || bill.Status != OrderStatus.Pending) return false;
            bill.Status = OrderStatus.Confirmed;
            bill.ConfirmedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkShippedAsync(int billId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && !b.IsDeleted);
            if (bill == null || bill.Status != OrderStatus.Confirmed) return false;
            bill.Status = OrderStatus.Shipped;
            bill.ShippedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkCompletedByUserAsync(int billId, int userId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && b.UserID == userId && !b.IsDeleted);
            if (bill == null || bill.Status != OrderStatus.Shipped) return false;
            bill.Status = OrderStatus.Completed;
            bill.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RequestCancelAsync(int billId, int userId, string? reason)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && b.UserID == userId && !b.IsDeleted);
            if (bill == null) return false;
            if (bill.Status == OrderStatus.Shipped || bill.Status == OrderStatus.Completed) return false;
            // allow when Pending or Confirmed
            bill.Status = OrderStatus.CancelRequested;
            bill.CancelReason = reason;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminCancelAsync(int billId, string? reason)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && !b.IsDeleted);
            if (bill == null) return false;
            if (bill.Status != OrderStatus.Pending) return false; // only before confirm
            bill.Status = OrderStatus.Cancelled;
            bill.CancelReason = reason;
            bill.CancelledAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveCancelAsync(int billId, string? reason)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && !b.IsDeleted);
            if (bill == null || bill.Status != OrderStatus.CancelRequested) return false;
            bill.Status = OrderStatus.Cancelled;
            bill.CancelReason = reason ?? bill.CancelReason;
            bill.CancelledAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectCancelAsync(int billId)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.BillID == billId && !b.IsDeleted);
            if (bill == null || bill.Status != OrderStatus.CancelRequested) return false;
            // revert to previous (heuristic): if confirmedAt exists, go back to Confirmed else Pending
            bill.Status = bill.ConfirmedAt.HasValue ? OrderStatus.Confirmed : OrderStatus.Pending;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
