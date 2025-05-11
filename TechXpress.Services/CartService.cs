using TechXpress.Data.Entities;
using TechXpress.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TechXpress.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _unitOfWork.GetCartWithItemsAsync(userId);
            if (cart != null)
                return cart;
            var newCart = new Cart { UserId = userId };
            await _unitOfWork.Carts.AddAsync(newCart);
            await _unitOfWork.SaveChangesAsync();
            return newCart;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null) return;
                cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity, UnitPrice = product.Price };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _unitOfWork.CartItems.Update(cartItem);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem != null)
            {
                _unitOfWork.CartItems.Remove(cartItem);
                await _unitOfWork.SaveChangesAsync();
            }
        }

      

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            foreach (var item in cart.CartItems.ToList())
            {
                _unitOfWork.CartItems.Remove(item);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
        }

        public async Task UpdateCartItemAsync(string userId, int productId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _unitOfWork.CartItems.Update(cartItem);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateCartItemQuantityAsync(string userId, int cartItemId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _unitOfWork.CartItems.Update(cartItem);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
} 