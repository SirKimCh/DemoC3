using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BanhMyIT.Controllers
{
    public class BillController : Controller
    {
        private readonly IBillService _billService;
        private readonly IUserService _userService;
        private readonly ILogger<BillController> _logger;
        public BillController(IBillService billService, IUserService userService, ILogger<BillController> logger)
        {
            _billService = billService;
            _userService = userService;
            _logger = logger;
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
            ViewBag.Users = await _userService.GetAllAsync();
            return View(new Bill());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bill bill)
        {
            if (ModelState.IsValid)
            {
                bill.TotalPrice = 0; // no details yet
                await _billService.AddAsync(bill);
                TempData["Success"] = "Bill created successfully";
                return RedirectToAction(nameof(Details), new { id = bill.BillID });
            }
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("Create Bill ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
            ViewBag.Users = await _userService.GetAllAsync();
            return View(bill);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
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
                TempData["Success"] = "Bill updated successfully";
                return RedirectToAction(nameof(Index));
            }
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("Edit Bill ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
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
            TempData["Success"] = "Bill deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Recalculate(int id)
        {
            await _billService.RecalculateTotalAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
