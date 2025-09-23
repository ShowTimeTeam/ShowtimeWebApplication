using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShowtimeWebApplication.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
