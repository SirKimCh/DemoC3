using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BanhMyIT.Data;

namespace BanhMyIT.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        private async Task<int?> GetDomainUserIdOrRedirectAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.DomainUserId is int id && id > 0)
                return id;
            TempData["ErrorMessage"] = "Vui lòng cập nhật hồ sơ trước khi đặt hàng.";
            return null;
        }

        public async Task<IActionResult> Index()
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            var cart = await _cartService.GetCartWithItemsAsync(userId.Value);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            await _cartService.AddItemAsync(userId.Value, productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartDetailId)
        {
            await _cartService.RemoveItemAsync(cartDetailId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            await _cartService.ClearCartAsync(userId.Value);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(PayMethod payMethod = PayMethod.Cash)
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            try
            {
                var bill = await _cartService.CheckoutAsync(userId.Value, payMethod);
                return RedirectToAction("Details", "Bill", new { id = bill.BillID });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
