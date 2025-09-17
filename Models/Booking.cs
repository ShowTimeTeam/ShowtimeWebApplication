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

        public DateTime Showtime { get; set; }

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
