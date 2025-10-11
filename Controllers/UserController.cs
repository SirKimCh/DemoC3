using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BanhMyIT.ViewModels;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace BanhMyIT.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly BanhMyITDbContext _db;
        public UserController(IUserService userService, ILogger<UserController> logger, BanhMyITDbContext db)
        {
            _userService = userService;
            _logger = logger;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _db.AppUsers.Include(u => u.Province).Include(u => u.District).ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Details(int id)
        {
            var user = await _db.AppUsers.Include(u => u.Province).Include(u => u.District).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return NotFound();
            return View(user);
        }
        private void LoadProvinceSelect(int? selectedId = null)
        {
            ViewBag.Provinces = _db.Provinces
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.ProvinceId.ToString(), Text = p.Name, Selected = selectedId.HasValue && selectedId.Value == p.ProvinceId })
                .ToList();
        }
        private void LoadDistrictSelect(int provinceId, int? selectedId = null)
        {
            ViewBag.Districts = _db.Districts.Where(d => d.ProvinceId == provinceId)
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem { Value = d.DistrictId.ToString(), Text = d.Name, Selected = selectedId.HasValue && selectedId.Value == d.DistrictId })
                .ToList();
        }
        public IActionResult Create()
        {
            LoadProvinceSelect();
            return View(new UserAddressViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var province = await _db.Provinces.FindAsync(model.ProvinceId);
                if (province == null)
                {
                    ModelState.AddModelError("ProvinceId", "Tỉnh/Thành phố không hợp lệ");
                }
                var district = await _db.Districts.FirstOrDefaultAsync(d => d.DistrictId == model.DistrictId && d.ProvinceId == model.ProvinceId);
                if (district == null)
                {
                    ModelState.AddModelError("DistrictId", "Quận/Huyện không hợp lệ");
                }
                if (ModelState.IsValid)
                {
                    var fullAddress = string.Join(", ", new[] { model.StreetAddress, district?.Name, province?.Name }.Where(s => !string.IsNullOrWhiteSpace(s)));
                    var user = new User
                    {
                        FirstMidName = model.FirstMidName,
                        LastName = model.LastName,
                        StreetAddress = model.StreetAddress,
                        ProvinceId = model.ProvinceId,
                        DistrictId = model.DistrictId,
                        Address = fullAddress
                    };
                    await _userService.AddAsync(user);
                    TempData["Success"] = "User created successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("Create User ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
            LoadProvinceSelect(model.ProvinceId == 0 ? null : model.ProvinceId);
            if (model.ProvinceId > 0)
                LoadDistrictSelect(model.ProvinceId, model.DistrictId);
            return View(model);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.AppUsers.Include(u => u.Province).Include(u => u.District).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return NotFound();
            var vm = new UserAddressViewModel
            {
                UserID = user.UserID,
                FirstMidName = user.FirstMidName,
                LastName = user.LastName,
                StreetAddress = user.StreetAddress,
                ProvinceId = user.ProvinceId,
                DistrictId = user.DistrictId,
                ProvinceName = user.Province?.Name,
                DistrictName = user.District?.Name
            };
            LoadProvinceSelect(vm.ProvinceId);
            if (vm.ProvinceId > 0)
                LoadDistrictSelect(vm.ProvinceId, vm.DistrictId);
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.AppUsers.FindAsync(model.UserID);
                if (user == null) return NotFound();
                var province = await _db.Provinces.FindAsync(model.ProvinceId);
                if (province == null)
                {
                    ModelState.AddModelError("ProvinceId", "Tỉnh/Thành phố không hợp lệ");
                }
                var district = await _db.Districts.FirstOrDefaultAsync(d => d.DistrictId == model.DistrictId && d.ProvinceId == model.ProvinceId);
                if (district == null)
                {
                    ModelState.AddModelError("DistrictId", "Quận/Huyện không hợp lệ");
                }
                if (ModelState.IsValid)
                {
                    user.FirstMidName = model.FirstMidName;
                    user.LastName = model.LastName;
                    user.StreetAddress = model.StreetAddress;
                    user.ProvinceId = model.ProvinceId;
                    user.DistrictId = model.DistrictId;
                    user.Address = string.Join(", ", new[] { model.StreetAddress, district?.Name, province?.Name }.Where(s => !string.IsNullOrWhiteSpace(s)));
                    await _userService.UpdateAsync(user);
                    TempData["Success"] = "User updated successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("Edit User ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
            LoadProvinceSelect(model.ProvinceId == 0 ? null : model.ProvinceId);
            if (model.ProvinceId > 0)
                LoadDistrictSelect(model.ProvinceId, model.DistrictId);
            return View(model);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.AppUsers.Include(u => u.Province).Include(u => u.District).FirstOrDefaultAsync(u => u.UserID == id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);
            TempData["Success"] = "User deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            if (provinceId <= 0) return Json(System.Array.Empty<object>());
            var districts = await _db.Districts.Where(d => d.ProvinceId == provinceId).OrderBy(d => d.Name).Select(d => new { d.DistrictId, d.Name }).ToListAsync();
            return Json(districts);
        }
    }
}
