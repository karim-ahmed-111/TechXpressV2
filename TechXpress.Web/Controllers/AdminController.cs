using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechXpress.Data.Entities;
using TechXpress.Services;
using Microsoft.AspNetCore.Identity;

namespace TechXpress.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IOrderService _orderService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(IProductService productService, ICategoryService categoryService, IOrderService orderService, IReviewService reviewService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _orderService = orderService;
            _reviewService = reviewService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            var lowStock = products.Where(p => p.Stock < 4).ToList();
            ViewBag.LowStock = lowStock;
            var orders = (await _orderService.GetAllOrdersAsync()).OrderByDescending(o => o.OrderDate).Take(5).ToList();
            ViewBag.RecentOrders = orders;
            var reviews = (await _reviewService.GetReviewsForProductAsync(0)).OrderByDescending(r => r.Date).Take(5).ToList();
            ViewBag.RecentReviews = reviews;
            return View();
        }

        // Product Management
        public async Task<IActionResult> Products()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            TempData["Debug"] = "CreateProduct action hit.";
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
                TempData["Debug"] += " ModelState is invalid: " + string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(product);
            }
            await _productService.AddProductAsync(product);
            TempData["Success"] = "Product created.";
            TempData["Debug"] += " Product added successfully.";
            return RedirectToAction("Products");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            TempData["Debug"] = "EditProduct action hit.";
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                Uri uriResult;
                bool isValidUrl = Uri.TryCreate(product.ImageUrl, UriKind.Absolute, out uriResult)
                                  && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!isValidUrl)
                {
                    ModelState.AddModelError("ImageUrl", "Please enter a valid image URL.");
                }
            }
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
                TempData["Debug"] += " ModelState is invalid: " + string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(product);
            }
            await _productService.UpdateProductAsync(product);
            TempData["Success"] = "Product updated.";
            TempData["Debug"] += " Product updated successfully.";
            return RedirectToAction("Products");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["Success"] = "Product deleted.";
            return RedirectToAction("Products");
        }

        // Category Management
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            await _categoryService.AddCategoryAsync(category);
            TempData["Success"] = "Category created.";
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            await _categoryService.UpdateCategoryAsync(category);
            TempData["Success"] = "Category updated.";
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            TempData["Success"] = "Category deleted.";
            return RedirectToAction("Categories");
        }

        // Orders Management
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            TempData["Success"] = "Order status updated.";
            return RedirectToAction("OrderDetails", new { id });
        }

        // Reviews Management
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(0); // 0 = all
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            TempData["Success"] = "Review deleted.";
            return RedirectToAction("Reviews");
        }

        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            // Get products purchased by the user (from orders)
            var orders = await _orderService.GetAllOrdersAsync();
            var userOrders = orders.Where(o => o.UserId == id).ToList();
            var purchasedProducts = userOrders.SelectMany(o => o.OrderDetails.Select(od => od.Product)).Distinct().ToList();
            ViewBag.PurchasedProducts = purchasedProducts;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            TempData["Success"] = "User deleted.";
            return RedirectToAction("Users");
        }
    }
}