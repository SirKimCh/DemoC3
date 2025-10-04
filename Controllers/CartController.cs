using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BanhMyIT.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            // For demo purposes, let's use a hard-coded user ID
            // In a real application, you would get this from authentication
            int userId = 1;
            
            var cart = await _cartService.GetCartWithItemsAsync(userId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // For demo purposes, let's use a hard-coded user ID
            int userId = 1;
            
            await _cartService.AddItemAsync(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartDetailId)
        {
            await _cartService.RemoveItemAsync(cartDetailId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            // For demo purposes, let's use a hard-coded user ID
            int userId = 1;
            
            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(PayMethod payMethod = PayMethod.Cash)
        {
            try
            {
                // For demo purposes, let's use a hard-coded user ID
                int userId = 1;
                
                var bill = await _cartService.CheckoutAsync(userId, payMethod);
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
