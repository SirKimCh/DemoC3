using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BanhMyIT.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.AddAsync(user);
                TempData["Success"] = "User created successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var kv in ModelState)
                    foreach (var err in kv.Value.Errors)
                        _logger.LogWarning("Create User ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
            }
            return View(user);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateAsync(user);
                TempData["Success"] = "User updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var kv in ModelState)
                    foreach (var err in kv.Value.Errors)
                        _logger.LogWarning("Edit User ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
            }
            return View(user);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
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
    }
}
