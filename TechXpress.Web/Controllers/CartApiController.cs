using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechXpress.Services;
using Microsoft.AspNetCore.Identity;
using TechXpress.Data.Entities;

namespace TechXpress.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartApiController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            return Ok(cart);
        }
    }
} 