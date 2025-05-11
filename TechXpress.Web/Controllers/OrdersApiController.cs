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
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrdersApiController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            var orders = await _orderService.GetOrdersForUserAsync(userId);
            return Ok(orders);
        }
    }
} 