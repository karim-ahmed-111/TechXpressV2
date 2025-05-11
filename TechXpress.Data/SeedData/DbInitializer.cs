using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechXpress.Data.Entities;

namespace TechXpress.Data.SeedData
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(TechXpressDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed roles
            var roles = new[] { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Seed admin user
            var adminEmail = "admin@techxpress.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    Address = "123 Admin St"
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Laptops", Description = "All kinds of laptops" },
                    new Category { Name = "Mobiles", Description = "Smartphones and accessories" },
                    new Category { Name = "Cameras", Description = "Digital cameras and gear" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed products
            if (!context.Products.Any())
            {
                var laptops = context.Categories.FirstOrDefault(c => c.Name == "Laptops");
                var mobiles = context.Categories.FirstOrDefault(c => c.Name == "Mobiles");
                var cameras = context.Categories.FirstOrDefault(c => c.Name == "Cameras");
                var theProducts = new List<Product>
                {
                    new Product { Name = "Dell XPS 13", Description = "13-inch laptop", Price = 999, Stock = 10, CategoryId = laptops.Id},
                    new Product { Name = "iPhone 15", Description = "Latest Apple iPhone", Price = 1199, Stock = 15, CategoryId = mobiles.Id},
                    new Product { Name = "Canon EOS R5", Description = "Mirrorless camera", Price = 3899, Stock = 5, CategoryId = cameras.Id}
                };
                context.Products.AddRange(theProducts);
                await context.SaveChangesAsync();
            }

            // Seed demo customers
            var demoCustomers = new List<(string Email, string FullName, string Address, string Password)>
            {
                ("alice@demo.com", "Alice Smith", "101 Main St", "Customer@123"),
                ("bob@demo.com", "Bob Johnson", "202 Oak Ave", "Customer@123"),
                ("carol@demo.com", "Carol Lee", "303 Pine Rd", "Customer@123")
            };
            var customerUsers = new List<ApplicationUser>();
            foreach (var (email, fullName, address, password) in demoCustomers)
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = fullName,
                        Address = address
                    };
                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "Customer");
                }
                customerUsers.Add(user ?? await userManager.FindByEmailAsync(email));
            }

            // Get products
            var products = context.Products.ToList();
            if (products.Count < 3) return; // Ensure products exist

            // Seed carts and cart items
            foreach (var customer in customerUsers)
            {
                if (!context.Carts.Any(c => c.UserId == customer.Id))
                {
                    var cart = new Cart { UserId = customer.Id };
                    context.Carts.Add(cart);
                    await context.SaveChangesAsync();
                    // Add cart items
                    var cartItems = new List<CartItem>
                    {
                        new CartItem { CartId = cart.Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = products[0].Price },
                        new CartItem { CartId = cart.Id, ProductId = products[1].Id, Quantity = 2, UnitPrice = products[1].Price }
                    };
                    context.CartItems.AddRange(cartItems);
                    await context.SaveChangesAsync();
                }
            }

            // Seed orders and order details
            var orderStatuses = new[] { "Pending", "Paid", "Shipped", "Cancelled" };
            int orderNum = 1;
            foreach (var customer in customerUsers)
            {
                if (!context.Orders.Any(o => o.UserId == customer.Id))
                {
                    var order = new Order
                    {
                        UserId = customer.Id,
                        OrderDate = DateTime.UtcNow.AddDays(-orderNum),
                        Status = orderStatuses[orderNum % orderStatuses.Length],
                        TotalAmount = products[0].Price * 1 + products[2].Price * 1,
                        StripePaymentId = $"stripe_test_{orderNum}",
                        ShippingAddress = customer.Address
                    };
                    context.Orders.Add(order);
                    await context.SaveChangesAsync();
                    var orderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { OrderId = order.Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = products[0].Price },
                        new OrderDetail { OrderId = order.Id, ProductId = products[2].Id, Quantity = 1, UnitPrice = products[2].Price }
                    };
                    context.OrderDetails.AddRange(orderDetails);
                    await context.SaveChangesAsync();
                    orderNum++;
                }
            }

            // Seed reviews
            var reviewComments = new[]
            {
                "Great product! Highly recommend.",
                "Good value for the price.",
                "Not what I expected, but works fine.",
                "Excellent quality and fast shipping.",
                "Would buy again."
            };
            int reviewNum = 0;
            foreach (var customer in customerUsers)
            {
                foreach (var product in products)
                {
                    if (!context.Reviews.Any(r => r.UserId == customer.Id && r.ProductId == product.Id))
                    {
                        var review = new Review
                        {
                            ProductId = product.Id,
                            UserId = customer.Id,
                            Rating = 4 + (reviewNum % 2),
                            Comment = reviewComments[reviewNum % reviewComments.Length],
                            Date = DateTime.UtcNow.AddDays(-reviewNum)
                        };
                        context.Reviews.Add(review);
                        reviewNum++;
                    }
                }
            }
            await context.SaveChangesAsync();
        }
    }
}