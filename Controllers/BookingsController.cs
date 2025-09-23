using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShowtimeWebApplication.Data;
using ShowtimeWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowtimeWebApplication.Controllers
{
    [Authorize]//All the actions requires login
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings.Include(b => b.Movie).Include(b => b.User);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: My Bookings (User's own bookings)
        public async Task<IActionResult> MyBookings()
        {
            var userId = _userManager.GetUserId(User);
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Movie);

            return View(await bookings.ToListAsync());
        }
        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Movie)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            //User only allow check thier own booking while admin could check all the bookings
            if (!User.IsInRole("Admin") && booking.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        public async Task<IActionResult> Create(int? movieId)
        {
            if (movieId == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            var booking = new Booking
            {
                MovieId = movieId.Value,
                Price = movie.Price
            };

            ViewData["MovieDetails"] = movie;

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", movieId);

            return View(booking);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,Cinema,SeatNumber,Showtime,Price,UserId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.UserId = _userManager.GetUserId(User);

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyBookings));
            }


            var movie = await _context.Movies.FindAsync(booking.MovieId);
            if (movie != null)
            {
                ViewData["MovieDetails"] = movie;
            }

            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", booking.MovieId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            // User only able to edit thier own bookings,admin could edit all the bookings
            if (booking.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            // For admin, show user dropdown with full names
            if (User.IsInRole("Admin"))
            {
                var users = await _context.Users
                    .Select(u => new { u.Id, u.FullName })
                    .ToListAsync();

                ViewBag.Users = new SelectList(users, "Id", "FullName", booking.UserId);
            }

            ViewData["MovieId"] = new SelectList(await _context.Movies.ToListAsync(), "Id", "Title", booking.MovieId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cinema,SeatNumber,Showtime,Price,UserId,MovieId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            // Get original booking to check permissions
            var originalBooking = await _context.Bookings.AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (originalBooking == null)
            {
                return NotFound();
            }

            // Check if user can edit this booking
            var currentUserId = _userManager.GetUserId(User);
            if (originalBooking.UserId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // If not admin, keep the original UserId
            if (!User.IsInRole("Admin"))
            {
                booking.UserId = originalBooking.UserId;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyBookings));
            }

            // Reload view data if validation fails
            var movies = await _context.Movies.ToListAsync();
            ViewData["MovieId"] = new SelectList(movies, "Id", "Title", booking.MovieId);

            if (User.IsInRole("Admin"))
            {
                ViewData["UserId"] = new SelectList(_context.Users.Select(u => new
                {
                    u.Id,
                    DisplayName = u.UserName + " (" + u.Email + ")"
                }), "Id", "DisplayName", booking.UserId);
            }

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Movie)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }
            //Check if it is the login user 
            if (booking.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                //Check if it is the login user 
                if (booking.UserId != _userManager.GetUserId(User))
                {
                    return Forbid();
                }
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

           
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
