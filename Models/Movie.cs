using System.ComponentModel.DataAnnotations;

namespace ShowtimeWebApplication.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        public Genre Genre { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000, ErrorMessage = "Price must be between 0.01 and 1000")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
        public int Duration { get; set; }

        // Showtimes stored as JSON string or separate table
        public string Showtimes { get; set; } // JSON format: [{"DateTime": "...", "Cinema": "...", "AvailableSeats": [...]}]

        public List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
