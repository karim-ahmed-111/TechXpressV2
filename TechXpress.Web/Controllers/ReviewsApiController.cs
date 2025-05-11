using Microsoft.AspNetCore.Mvc;
using TechXpress.Services;

namespace TechXpress.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsApiController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewsApiController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }
    }
} 