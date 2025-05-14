using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Data.Entities;
using TechXpress.Services;

namespace TechXpress.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryService _categoryService;
        public ProductsController(IProductService productService, IReviewService reviewService, IOrderService orderService, UserManager<ApplicationUser> userManager, ICategoryService categoryService)
        {
            _productService = productService;
            _reviewService = reviewService;
            _orderService = orderService;
            _userManager = userManager;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productService.GetAllProductsAsync();
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            }
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            var reviews = await _reviewService.GetReviewsForProductAsync(id);
            ViewBag.Reviews = reviews;
            bool canReview = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                canReview = await _reviewService.CanUserReviewProductAsync(userId, id);
            }
            ViewBag.CanReview = canReview;
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment, string? anonymousName)
        {
            string? userId = null;
            if (User.Identity.IsAuthenticated)
            {
                userId = _userManager.GetUserId(User);
            }
            else if (!string.IsNullOrWhiteSpace(anonymousName))
            {
                comment = $"[By: {anonymousName}] " + comment;
            }
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                Date = DateTime.UtcNow
            };
            await _reviewService.AddReviewAsync(review);
            TempData["Success"] = "Review submitted.";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}