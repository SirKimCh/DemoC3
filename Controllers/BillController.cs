using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using BanhMyIT.Data;
using BanhMyIT.ViewModels;

namespace BanhMyIT.Controllers
{
    public class BillController : Controller
    {
        private readonly IBillService _billService;
        private readonly IUserService _userService;
        private readonly ILogger<BillController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public BillController(IBillService billService, IUserService userService, ILogger<BillController> logger, UserManager<ApplicationUser> userManager)
        {
            _billService = billService;
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index(string? search, OrderStatus? status, string? sortBy = "created", bool desc = false, int page = 1, int pageSize = 10)
        {
            var (items, total) = await _billService.QueryAsync(search, status, sortBy, desc, page, pageSize);
            var vm = new BillListViewModel
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                Search = search,
                Status = status,
                SortBy = sortBy,
                Desc = desc
            };
            ViewData.Model = vm;
            return View();
        }

        private async Task<int?> GetDomainUserIdOrRedirectAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.DomainUserId is int id && id > 0) return id;
            TempData["ErrorMessage"] = "Vui lòng cập nhật hồ sơ trước.";
            return null;
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
            if (!User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                var userId = await GetDomainUserIdOrRedirectAsync();
                if (!userId.HasValue) return RedirectToAction("Profile", "Account");
                if (bill.UserID != userId.Value) return Forbid();
            }
            return View(bill);
        }
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _userService.GetAllAsync();
            return View(new Bill());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
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
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
            ViewBag.Users = await _userService.GetAllAsync();
            return View(bill);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
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
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var bill = await _billService.GetByIdAsync(id);
            if (bill == null) return NotFound();
            return View(bill);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _billService.DeleteAsync(id);
            TempData["Success"] = "Bill deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Recalculate(int id)
        {
            await _billService.RecalculateTotalAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize]
        public async Task<IActionResult> My()
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            var list = (await _billService.GetByUserAsync(userId.Value)).ToList();
            var vm = new BillListViewModel
            {
                Items = list,
                TotalCount = list.Count,
                Page = 1,
                PageSize = list.Count == 0 ? 10 : list.Count,
                IsMyList = true
            };
            ViewData.Model = vm;
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Confirm(int id)
        {
            var ok = await _billService.ConfirmAsync(id);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Đã xác thực đơn hàng." : "Không thể xác thực đơn hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Ship(int id)
        {
            var ok = await _billService.MarkShippedAsync(id);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Đã chuyển trạng thái Shipped." : "Không thể chuyển trạng thái Shipped.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AdminCancel(int id, string? reason)
        {
            var ok = await _billService.AdminCancelAsync(id, reason);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Đã hủy đơn hàng." : "Không thể hủy đơn hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveCancel(int id, string? reason)
        {
            var ok = await _billService.ApproveCancelAsync(id, reason);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Đã chấp nhận hủy đơn." : "Không thể chấp nhận hủy.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectCancel(int id)
        {
            var ok = await _billService.RejectCancelAsync(id);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Đã từ chối yêu cầu hủy." : "Không thể từ chối yêu cầu hủy.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            var ok = await _billService.MarkCompletedByUserAsync(id, userId.Value);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Cảm ơn bạn! Đơn hàng đã hoàn thành." : "Không thể xác nhận đã nhận hàng.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RequestCancel(int id, string? reason)
        {
            var userId = await GetDomainUserIdOrRedirectAsync();
            if (!userId.HasValue) return RedirectToAction("Profile", "Account");
            var ok = await _billService.RequestCancelAsync(id, userId.Value, reason);
            TempData[ok ? "Success" : "ErrorMessage"] = ok ? "Đã gửi yêu cầu hủy. Vui lòng chờ duyệt." : "Không thể gửi yêu cầu hủy.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
