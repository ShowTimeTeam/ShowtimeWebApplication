using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShowtimeWebApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public override string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email format")]
        public override string Email { get; set; }

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
