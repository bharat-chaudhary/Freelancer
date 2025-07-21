using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Freelancer.Data;
using Freelancer.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Freelancer.Controllers
{
    public class EmployerController : Controller
    {
        private readonly AppDbContext _context;

        public EmployerController(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public IActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString("Username") ?? "Employer";
            var profileImage = HttpContext.Session.GetString("ProfileImage");

            // If profile image is not set in session, use a default image
            if (string.IsNullOrEmpty(profileImage))
            {
                profileImage = "/images/contacts_16076050.png"; // Default fallback image
                HttpContext.Session.SetString("ProfileImage", profileImage); // Store in session
            }

            ViewBag.Username = username;
            ViewBag.ProfileImage = profileImage; // Pass the image path dynamically

            return View();
        }

        // Profile View
        [HttpGet]
        public IActionResult Profile()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // Edit Profile View
        public IActionResult EditProfile()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        // Edit Profile - POST
        [HttpPost]
        public IActionResult EditProfile(User model, IFormFile profilePicture)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
            if (user == null)
                return RedirectToAction("Login");

            user.FullName = model.FullName;
            user.Bio = model.Bio;
            user.ContactNumber = model.ContactNumber;

            // Profile Picture Upload Logic
            if (profilePicture != null && profilePicture.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                // Ensure directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = $"{user.UserId}{Path.GetExtension(profilePicture.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    profilePicture.CopyTo(stream);
                }

                user.ProfilePicture = "/images/" + fileName;
                HttpContext.Session.SetString("ProfilePicture", user.ProfilePicture);
            }

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }

      
    }
}