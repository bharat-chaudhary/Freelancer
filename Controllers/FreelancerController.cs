using Freelancer.Data;
using Freelancer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Freelancer.Controllers
{
    public class FreelancerController : Controller
    {
        private readonly AppDbContext _context;

        public FreelancerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString("Username") ?? "Freelancer";
            var profileImage = HttpContext.Session.GetString("ProfilePicture") ?? "/images/contacts_16076050.png";

            ViewBag.Username = username;
            ViewBag.ProfileImage = profileImage;

            HttpContext.Session.SetString("ProfilePicture", profileImage);

            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user == null ? RedirectToAction("Login", "Account") : View(user);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            return user == null ? RedirectToAction("Login", "Account") : View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(User model, IFormFile profilePicture)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);
            if (user == null) return RedirectToAction("Login", "Account");

            user.FullName = model.FullName;
            user.Bio = model.Bio;
            user.ContactNumber = model.ContactNumber;

            if (profilePicture != null && profilePicture.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{user.UserId}{Path.GetExtension(profilePicture.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await profilePicture.CopyToAsync(stream);

                user.ProfilePicture = "/images/" + fileName;
                HttpContext.Session.SetString("ProfilePicture", user.ProfilePicture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Profile");
        }

        public IActionResult ViewAllProjects(string? searchTerm)
        {
            var projectsQuery = _context.Projects
                .Where(p => p.Status == "Open");

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                projectsQuery = projectsQuery.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Language.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
            }

            var projects = projectsQuery
                .OrderByDescending(p => p.Start_Date)
                .ToList();

            return View(projects);
        }

        public IActionResult ProjectDetails(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.ProjectId == id);
            return project == null ? NotFound() : View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeProject(int projectId, decimal bidAmount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to place a bid.";
                return RedirectToAction("Login", "Account");
            }

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null || project.Status != "Open")
            {
                TempData["ErrorMessage"] = "Project is not available.";
                return RedirectToAction("ViewAllProjects");
            }

            bool alreadyBid = await _context.Bids
                .AnyAsync(b => b.Project_ID == projectId && b.UserId == userId.Value);

            if (alreadyBid)
            {
                TempData["ErrorMessage"] = "You have already placed a bid.";
                return RedirectToAction("ViewAllProjects");
            }

            var bid = new Bid
            {
                Project_ID = projectId,
                BidAmount = bidAmount,
                Bid_Date = DateTime.Now,
                Assign = "No",
                UserId = userId.Value
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your bid has been placed successfully!";
            return RedirectToAction("ViewAllProjects");
        }

        [HttpGet]
        public IActionResult LiveProject()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var projects = _context.Projects
                .Where(p => p.TakenByFreelancerId == userId.Value)
                .ToList();

            return View("LiveProject", projects);
        }

        [HttpPost]
        public IActionResult CompleteProject(int projectId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var project = _context.Projects
                .FirstOrDefault(p => p.ProjectId == projectId && p.TakenByFreelancerId == userId.Value);

            if (project != null && project.Status == "Taken")
            {
                project.Status = "Completed";
                _context.SaveChanges();
            }

            return RedirectToAction("LiveProject");
        }

        [HttpGet]
        public IActionResult ProjectHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var completedProjects = _context.Projects
                .Where(p => p.TakenByFreelancerId == userId.Value && p.Status == "Completed")
                .ToList();

            return View(completedProjects);
        }
    }
}
