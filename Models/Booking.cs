using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShowtimeWebApplication.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema selection is required")]
        public string Cinema { get; set; }

        [Required(ErrorMessage = "Seat number is required")]
        [RegularExpression(@"^[A-Z]\d{1,2}$", ErrorMessage = "Seat format must be like A1, B12, etc.")]
        public string SeatNumber { get; set; }

        [Required(ErrorMessage = "Showtime is required")]
        public DateTime Showtime { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000, ErrorMessage = "Price must be between 0.01 and 1000")]
        public decimal Price { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
}
