using Microsoft.AspNetCore.Identity;
using ShowtimeWebApplication.Models;
using System.Text.Json;

namespace ShowtimeWebApplication.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();

            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
            await SeedCustomerUsers(userManager);
            await SeedMovies(context);
            await SeedAdminBookings(context, userManager);
            await SeedCustomerBookings(context, userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@showtime.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    PhoneNumber = "555-0100",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        private static async Task SeedCustomerUsers(UserManager<ApplicationUser> userManager)
        {
            var customerEmail = "customer@showtime.com";
            var customerUser = await userManager.FindByEmailAsync(customerEmail);

            if (customerUser == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    FullName = "John Customer",
                    PhoneNumber = "555-0200",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(customer, "Customer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }
            }

            var customer2Email = "customer2@showtime.com";
            var customer2User = await userManager.FindByEmailAsync(customer2Email);

            if (customer2User == null)
            {
                var customer2 = new ApplicationUser
                {
                    UserName = customer2Email,
                    Email = customer2Email,
                    FullName = "Jane Customer",
                    PhoneNumber = "555-0300",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(customer2, "Customer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer2, "Customer");
                }
            }
        }

        private static async Task SeedMovies(ApplicationDbContext context)
        {
            if (!context.Movies.Any())
            {
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "The Matrix",
                        Description = "A computer hacker learns from mysterious rebels about the true nature of his reality.",
                        Genre = Genre.SciFi,
                        Price = 12.99m,
                        Duration = 136,
                        Showtimes = JsonSerializer.Serialize(new[]
                        {
                            new { DateTime = new DateTime(2024, 1, 15, 18, 0, 0), Cinema = "Cinema City 1", AvailableSeats = new[] { "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3" } },
                            new { DateTime = new DateTime(2024, 1, 16, 20, 0, 0), Cinema = "Cinema City 2", AvailableSeats = new[] { "A4", "A5", "A6", "B4", "B5", "B6", "C4", "C5", "C6" } }
                        })
                    },
                    new Movie
                    {
                        Title = "The Dark Knight",
                        Description = "Batman sets out to dismantle the remaining criminal organizations that plague the streets.",
                        Genre = Genre.Action,
                        Price = 14.99m,
                        Duration = 152,
                        Showtimes = JsonSerializer.Serialize(new[]
                        {
                            new { DateTime = new DateTime(2024, 1, 15, 19, 0, 0), Cinema = "Cinema City 1", AvailableSeats = new[] { "D1", "D2", "D3", "E1", "E2", "E3", "F1", "F2", "F3" } },
                            new { DateTime = new DateTime(2024, 1, 17, 21, 0, 0), Cinema = "Cinema City 3", AvailableSeats = new[] { "D4", "D5", "D6", "E4", "E5", "E6", "F4", "F5", "F6" } }
                        })
                    },
                    new Movie
                    {
                        Title = "Inception",
                        Description = "A thief who steals corporate secrets through dream-sharing technology.",
                        Genre = Genre.Thriller,
                        Price = 13.99m,
                        Duration = 148,
                        Showtimes = JsonSerializer.Serialize(new[]
                        {
                            new { DateTime = new DateTime(2024, 1, 16, 17, 0, 0), Cinema = "Cinema City 2", AvailableSeats = new[] { "G1", "G2", "G3", "H1", "H2", "H3", "I1", "I2", "I3" } },
                            new { DateTime = new DateTime(2024, 1, 18, 19, 0, 0), Cinema = "Cinema City 1", AvailableSeats = new[] { "G4", "G5", "G6", "H4", "H5", "H6", "I4", "I5", "I6" } }
                        })
                    },
                    new Movie
                    {
                        Title = "Toy Story",
                        Description = "A cowboy doll is profoundly threatened when a new spaceman figure supplants him.",
                        Genre = Genre.Animation,
                        Price = 9.99m,
                        Duration = 81,
                        Showtimes = JsonSerializer.Serialize(new[]
                        {
                            new { DateTime = new DateTime(2024, 1, 18, 14, 0, 0), Cinema = "Cinema City 3", AvailableSeats = new[] { "J1", "J2", "J3", "K1", "K2", "K3", "L1", "L2", "L3" } },
                            new { DateTime = new DateTime(2024, 1, 19, 16, 0, 0), Cinema = "Cinema City 2", AvailableSeats = new[] { "J4", "J5", "J6", "K4", "K5", "K6", "L4", "L5", "L6" } }
                        })
                    },
                    new Movie
                    {
                        Title = "The Shawshank Redemption",
                        Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption.",
                        Genre = Genre.Drama,
                        Price = 11.99m,
                        Duration = 142,
                        Showtimes = JsonSerializer.Serialize(new[]
                        {
                            new { DateTime = new DateTime(2024, 1, 17, 18, 0, 0), Cinema = "Cinema City 1", AvailableSeats = new[] { "M1", "M2", "M3", "N1", "N2", "N3", "O1", "O2", "O3" } },
                            new { DateTime = new DateTime(2024, 1, 19, 20, 0, 0), Cinema = "Cinema City 3", AvailableSeats = new[] { "M4", "M5", "M6", "N4", "N5", "N6", "O4", "O5", "O6" } }
                        })
                    }
                };

                context.Movies.AddRange(movies);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAdminBookings(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@showtime.com");
            var movies = context.Movies.ToList();

            if (adminUser != null && movies.Any() && !context.Bookings.Any(b => b.UserId == adminUser.Id))
            {
                var bookings = new List<Booking>
                {
                    new Booking
                    {
                        Cinema = "Cinema City 1",
                        SeatNumber = "A1",
                        Showtime = new DateTime(2024, 1, 15, 18, 0, 0),
                        Price = 12.99m,
                        UserId = adminUser.Id,
                        MovieId = movies[0].Id
                    },
                    new Booking
                    {
                        Cinema = "Cinema City 1",
                        SeatNumber = "D1",
                        Showtime = new DateTime(2024, 1, 15, 19, 0, 0),
                        Price = 14.99m,
                        UserId = adminUser.Id,
                        MovieId = movies[1].Id
                    }
                };

                context.Bookings.AddRange(bookings);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedCustomerBookings(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var customerUser = await userManager.FindByEmailAsync("customer@showtime.com");
            var customer2User = await userManager.FindByEmailAsync("customer2@showtime.com");
            var movies = context.Movies.ToList();

            if (customerUser != null && customer2User != null && movies.Any())
            {
                var bookings = new List<Booking>();

                if (!context.Bookings.Any(b => b.UserId == customerUser.Id))
                {
                    bookings.Add(new Booking
                    {
                        Cinema = "Cinema City 1",
                        SeatNumber = "A2",
                        Showtime = new DateTime(2024, 1, 15, 18, 0, 0),
                        Price = 12.99m,
                        UserId = customerUser.Id,
                        MovieId = movies[0].Id
                    });

                    bookings.Add(new Booking
                    {
                        Cinema = "Cinema City 2",
                        SeatNumber = "G1",
                        Showtime = new DateTime(2024, 1, 16, 17, 0, 0),
                        Price = 13.99m,
                        UserId = customerUser.Id,
                        MovieId = movies[2].Id
                    });
                }

                if (!context.Bookings.Any(b => b.UserId == customer2User.Id))
                {
                    bookings.Add(new Booking
                    {
                        Cinema = "Cinema City 1",
                        SeatNumber = "A3",
                        Showtime = new DateTime(2024, 1, 15, 18, 0, 0),
                        Price = 12.99m,
                        UserId = customer2User.Id,
                        MovieId = movies[0].Id
                    });

                    bookings.Add(new Booking
                    {
                        Cinema = "Cinema City 3",
                        SeatNumber = "J1",
                        Showtime = new DateTime(2024, 1, 18, 14, 0, 0),
                        Price = 9.99m,
                        UserId = customer2User.Id,
                        MovieId = movies[3].Id
                    });
                }

                if (bookings.Any())
                {
                    context.Bookings.AddRange(bookings);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}