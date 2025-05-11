using System.Collections.Generic;
using TechXpress.Data.Entities;

namespace TechXpress.Web.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
} 