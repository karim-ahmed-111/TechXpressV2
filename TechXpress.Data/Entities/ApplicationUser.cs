using Microsoft.AspNetCore.Identity;

namespace TechXpress.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        // PhoneNumber is inherited from IdentityUser
        // Add more profile fields as needed
    }
} 