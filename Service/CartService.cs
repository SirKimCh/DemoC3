using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.EntityFrameworkCore;

namespace BanhMyIT.Service
{
    public class CartService : ICartService
    {
        private readonly BanhMyITDbContext _context;
        private readonly IBillService _billService;

        public CartService(BanhMyITDbContext context, IBillService billService)
        {
            _context = context;
            _billService = billService;
        }

        public async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.UserID == userId);
            if (cart == null)
            {
                cart = new Cart { UserID = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart?> GetCartWithItemsAsync(int userId)
        {
            return await _context.Carts.Include(c => c.CartDetails).ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);
        }

        public async Task AddItemAsync(int userId, int productId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId) ??
                          throw new InvalidOperationException("Product not found");
            var cart = await GetOrCreateCartAsync(userId);
            var existing =
                await _context.CartDetails.FirstOrDefaultAsync(d =>
                    d.CartId == cart.CartId && d.ProductID == productId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                existing.SubTotal = existing.Quantity * existing.UnitPrice;
            }
            else
            {
                var detail = new CartDetail
                {
                    CartId = cart.CartId, ProductID = productId, Quantity = quantity, UnitPrice = product.Price,
                    SubTotal = product.Price * quantity
                };
                _context.CartDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int cartDetailId)
        {
            var detail = await _context.CartDetails.FindAsync(cartDetailId);
            if (detail == null) return;
            _context.CartDetails.Remove(detail);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.UserID == userId);
            if (cart == null) return;
            _context.CartDetails.RemoveRange(cart.CartDetails);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task<Bill> CheckoutAsync(int userId, PayMethod? payMethod)
        {
            var cart = await GetCartWithItemsAsync(userId) ?? throw new InvalidOperationException("Cart not found");
            if (cart.CartDetails.Count == 0) throw new InvalidOperationException("Cart empty");
            return await _billService.CreateFromCartAsync(cart.CartId, payMethod);
        }
    }
}