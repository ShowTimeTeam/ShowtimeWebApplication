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
        public async Task<IActionResult> CreateAsync(int? movieId)
        {
            var movies = await _context.Movies.ToListAsync();
            ViewData["MovieId"] = new SelectList(movies, "Id", "Title", movieId);
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Cinema,SeatNumber,Showtime,Price,UserId,MovieId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.UserId = _userManager.GetUserId(User);
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyBookings));
            }
            var movies = await _context.Movies.ToListAsync();
            ViewData["MovieId"] = new SelectList(movies, "Id", "Title", booking.MovieId);
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

            // User only able to edit thier own bookings
            if (booking.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            var movies = await _context.Movies.ToListAsync();
            ViewData["MovieId"] = new SelectList(movies, "Id", "Title", booking.MovieId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cinema,SeatNumber,Showtime,Price,UserId,MovieId")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }
            //Check if it is the login user 
            if (booking.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
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
            var movies = await _context.Movies.ToListAsync();
            ViewData["MovieId"] = new SelectList(movies, "Id", "Title", booking.MovieId);
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
