using System.Threading.Tasks;
using BanhMyIT.Data;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BanhMyIT.ViewModels;

namespace BanhMyIT.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BanhMyITDbContext _db;
        
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, BanhMyITDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Forbid();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (User.Identity?.IsAuthenticated == true)
                return Forbid();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Username and password (min 6 chars) are required.");
                return View();
            }

            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Assign default role User
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStaff()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Username and password (min 6 chars) are required.");
                return View();
            }
            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");
                TempData["Success"] = "Staff account created";
                return RedirectToAction("Index", "Home");
            }
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login");

            UserAddressViewModel vm = new();
            if (appUser.DomainUserId is int id && id > 0)
            {
                var domainUser = await _db.AppUsers.Include(u => u.Province).Include(u => u.District).FirstOrDefaultAsync(u => u.UserID == id);
                if (domainUser != null)
                {
                    vm = new UserAddressViewModel
                    {
                        UserID = domainUser.UserID,
                        FirstMidName = domainUser.FirstMidName,
                        LastName = domainUser.LastName,
                        StreetAddress = domainUser.StreetAddress,
                        ProvinceId = domainUser.ProvinceId,
                        DistrictId = domainUser.DistrictId,
                        ProvinceName = domainUser.Province?.Name,
                        DistrictName = domainUser.District?.Name
                    };
                }
            }
            await LoadSelectsAsync(vm.ProvinceId == 0 ? null : vm.ProvinceId, vm.DistrictId == 0 ? null : vm.DistrictId);
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserAddressViewModel model)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                var province = await _db.Provinces.FindAsync(model.ProvinceId);
                if (province == null)
                    ModelState.AddModelError("ProvinceId", "Tỉnh/Thành phố không hợp lệ");
                var district = await _db.Districts.FirstOrDefaultAsync(d => d.DistrictId == model.DistrictId && d.ProvinceId == model.ProvinceId);
                if (district == null)
                    ModelState.AddModelError("DistrictId", "Quận/Huyện không hợp lệ");
                if (ModelState.IsValid)
                {
                    User domainUser;
                    if (appUser.DomainUserId is int id && id > 0)
                    {
                        domainUser = await _db.AppUsers.FindAsync(id) ?? new User();
                        if (domainUser.UserID == 0)
                        {
                            _db.AppUsers.Add(domainUser);
                        }
                    }
                    else
                    {
                        domainUser = new User();
                        _db.AppUsers.Add(domainUser);
                    }
                    domainUser.FirstMidName = model.FirstMidName;
                    domainUser.LastName = model.LastName;
                    domainUser.StreetAddress = model.StreetAddress;
                    domainUser.ProvinceId = model.ProvinceId;
                    domainUser.DistrictId = model.DistrictId;
                    domainUser.Address = string.Join(", ", new[] { model.StreetAddress, district?.Name, province?.Name }.Where(s => !string.IsNullOrWhiteSpace(s)));
                    await _db.SaveChangesAsync();

                    if (!(appUser.DomainUserId is int id2) || id2 <= 0)
                    {
                        appUser.DomainUserId = domainUser.UserID;
                        await _userManager.UpdateAsync(appUser);
                    }
                    TempData["Success"] = "Cập nhật hồ sơ thành công";
                    return RedirectToAction("Index", "Home");
                }
            }
            await LoadSelectsAsync(model.ProvinceId == 0 ? null : model.ProvinceId, model.DistrictId == 0 ? null : model.DistrictId);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            if (provinceId <= 0) return Json(System.Array.Empty<object>());
            var districts = await _db.Districts.Where(d => d.ProvinceId == provinceId).OrderBy(d => d.Name).Select(d => new { d.DistrictId, d.Name }).ToListAsync();
            return Json(districts);
        }

        private async Task LoadSelectsAsync(int? provinceId, int? districtId)
        {
            ViewBag.Provinces = await _db.Provinces
                .OrderBy(p => p.Name)
                .Select(p => new SelectListItem { Value = p.ProvinceId.ToString(), Text = p.Name, Selected = provinceId.HasValue && provinceId.Value == p.ProvinceId })
                .ToListAsync();
            if (provinceId.HasValue && provinceId.Value > 0)
            {
                ViewBag.Districts = await _db.Districts.Where(d => d.ProvinceId == provinceId)
                    .OrderBy(d => d.Name)
                    .Select(d => new SelectListItem { Value = d.DistrictId.ToString(), Text = d.Name, Selected = districtId.HasValue && districtId.Value == d.DistrictId })
                    .ToListAsync();
            }
            else
            {
                ViewBag.Districts = new List<SelectListItem>();
            }
        }
    }
}
