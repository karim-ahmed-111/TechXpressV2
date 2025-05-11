using System;
using System.Collections.Generic;

namespace TechXpress.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Paid, Shipped, Cancelled, etc.
        public decimal TotalAmount { get; set; }
        public string StripePaymentId { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
} 