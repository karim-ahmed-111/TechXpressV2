using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using TechXpress.Services;
using Microsoft.AspNetCore.Identity;
using TechXpress.Data.Entities;

namespace TechXpress.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrdersController(IOrderService orderService, ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");
            var orders = await _orderService.GetOrdersForUserAsync(userId);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string shippingAddress)
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            var domain = Request.Scheme + "://" + Request.Host.Value;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cart.CartItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product?.Name ?? "Product"
                        }
                    },
                    Quantity = item.Quantity
                }).ToList(),
                Mode = "payment",
                SuccessUrl = domain + "/Orders/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "/Orders/Cancel"
            };
            var service = new SessionService();
            var session = service.Create(options);
            TempData["ShippingAddress"] = shippingAddress;
            TempData["StripeSessionId"] = session.Id;
            return Redirect(session.Url);
        }

        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            var userId = _userManager.GetUserId(User);
            var shippingAddress = TempData["ShippingAddress"]?.ToString() ?? "";
            var sessionService = new SessionService();
            var session = sessionService.Get(session_id);
            if (session.PaymentStatus == "paid")
            {
                await _orderService.PlaceOrderAsync(userId, shippingAddress, session.PaymentIntentId);
                TempData["Success"] = "Order placed and payment successful!";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Payment not completed.";
            return RedirectToAction("Checkout");
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            TempData["Error"] = "Payment was cancelled.";
            return RedirectToAction("Checkout");
        }
    }
} 