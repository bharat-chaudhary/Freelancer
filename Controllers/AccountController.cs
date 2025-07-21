using Freelancer.Data;
using Freelancer.Models;
using Freelancer.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Freelancer.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {


            if (!ModelState.IsValid)
                return View(model);

            string email = model.Email?.Trim().ToLower();
            string password = model.Password;

            if (email == "admin@freelancer.com" && password == "Admin@12345")
            {
                HttpContext.Session.SetInt32("UserId", 0); // Admin ID is 0
                HttpContext.Session.SetString("Username", "Admin");
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("ProfilePicture", "/images/profile.png");
                return RedirectToAction("Dashboard", "Admin");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // ✅ Fixed: set UserId as integer
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("ProfilePicture", user.ProfilePicture ?? "/images/profile.png");

            var audit = new Audit
            {
                User_id = user.UserId,
                Login_time = DateTime.Now,
                Logout_time = null
            };
            _context.Audits.Add(audit);
            _context.SaveChanges();

            return user.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Freelancer" => RedirectToAction("Dashboard", "Freelancer"),
                "Employer" => RedirectToAction("Dashboard", "Employer"),
                _ => RedirectToAction("Login")
            };
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string email = model.Email?.Trim().ToLower();

            if (_context.Users.Any(u => u.Email.ToLower() == email))
            {
                ModelState.AddModelError("", "Email already in use.");
                return View(model);
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(model.Password, salt);

            var user = new User
            {
                Username = model.Username,
                Email = email,
                Password = $"{salt}${hashedPassword}",
                Role = model.Role,
                ProfilePicture = "/images/profile.png"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user == null ? RedirectToAction("Login") : View(user);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user == null ? RedirectToAction("Login") : View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(User model, IFormFile profilePicture)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
            if (user == null) return RedirectToAction("Login");

            user.FullName = model.FullName;
            user.ContactNumber = model.ContactNumber;
            user.Bio = model.Bio;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(profilePicture.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    profilePicture.CopyTo(stream);

                user.ProfilePicture = "/images/" + fileName;
                HttpContext.Session.SetString("ProfilePicture", user.ProfilePicture);
            }

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var lastAudit = _context.Audits
                    .Where(a => a.User_id == userId && a.Logout_time == null)
                    .OrderByDescending(a => a.Login_time)
                    .FirstOrDefault();

                if (lastAudit != null)
                {
                    lastAudit.Logout_time = DateTime.Now;
                    _context.SaveChanges();
                }
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Utilities
        private static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        private static string HashPassword(string password, string salt)
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(salt + password);
            byte[] hashBytes = SHA256.HashData(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }

        private static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            if (storedPassword.Contains("$"))
            {
                var parts = storedPassword.Split('$');
                if (parts.Length != 2) return false;

                string salt = parts[0];
                string storedHash = parts[1];
                string enteredHash = HashPassword(enteredPassword, salt);

                return enteredHash.Equals(storedHash);
            }

            return enteredPassword == storedPassword;
        }
    }
}
