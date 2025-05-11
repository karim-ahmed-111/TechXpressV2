using System.ComponentModel.DataAnnotations;

namespace TechXpress.Web.Models
{
    public class EditProfileViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
} 