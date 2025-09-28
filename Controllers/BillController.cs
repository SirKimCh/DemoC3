using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BanhMyIT.Controllers
{
    public class BillController : Controller
    {
        private readonly IBillService _billService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        public BillController(IBillService billService, IProductService productService, IUserService userService)
        {
            _billService = billService;
            _productService = productService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var bills = await _billService.GetAllAsync();
            return View(bills);
        }
        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _productService.GetAllAsync();
            ViewBag.Users = await _userService.GetAllAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bill bill)
        {
            if (ModelState.IsValid)
            {
                await _billService.AddAsync(bill);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _productService.GetAllAsync();
            ViewBag.Users = await _userService.GetAllAsync();
            return View(bill);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
            ViewBag.Products = await _productService.GetAllAsync();
            ViewBag.Users = await _userService.GetAllAsync();
            return View(bill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Bill bill)
        {
            if (ModelState.IsValid)
            {
                await _billService.UpdateAsync(bill);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Products = await _productService.GetAllAsync();
            ViewBag.Users = await _userService.GetAllAsync();
            return View(bill);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _billService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

