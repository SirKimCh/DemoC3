using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace BanhMyIT.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly BanhMyITDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, BanhMyITDbContext context, ILogger<ProductController> logger)
        {
            _productService = productService;
            _context = context;
            _logger = logger;
        }

        private void PopulateCategories(int? selectedId = null)
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryID", "Name", selectedId);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            ViewBag.Users = _context.Users.Select(u => new { u.UserID, FullName = u.FirstMidName + " " + u.LastName }).ToList();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            PopulateCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddAsync(product);
                TempData["Success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        _logger.LogWarning("Create Product ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                    }
                }
            }
            PopulateCategories(product.CategoryID);
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            PopulateCategories(product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.ProductID) return BadRequest();
            if (ModelState.IsValid)
            {
                await _productService.UpdateAsync(product);
                TempData["Success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var kv in ModelState)
                {
                    foreach (var err in kv.Value.Errors)
                    {
                        _logger.LogWarning("Edit Product ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                    }
                }
            }
            PopulateCategories(product.CategoryID);
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["Success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}