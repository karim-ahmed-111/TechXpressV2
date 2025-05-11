using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Data.Entities;
using TechXpress.Services;
using Microsoft.AspNetCore.Identity;

namespace TechXpress.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductsController(IProductService productService, IReviewService reviewService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _reviewService = reviewService;
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
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
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var userId = _userManager.GetUserId(User);
            if (!await _reviewService.CanUserReviewProductAsync(userId, productId))
            {
                TempData["Error"] = "You can only review products you have purchased.";
                return RedirectToAction("Details", new { id = productId });
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