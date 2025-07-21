using Freelancer.Data;
using Freelancer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Freelancer.Controllers
{
    public class ProjectController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        // Helper: Programming languages
        private List<string> GetLanguageList()
        {
            return new List<string>
            {
                "C#", "Java", "Python", "JavaScript", "PHP", "Ruby", "Go", "Swift", "Kotlin", "TypeScript"
            };
        }

        // GET: New Project Form
        public IActionResult NewProject()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Languages = GetLanguageList();
            return View();
        }

        // POST: Create New Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewProject(Project project)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "You must be logged in.");
                ViewBag.Languages = GetLanguageList();
                return View(project);
            }

            if (ModelState.IsValid)
            {
                if (project.End_Date < DateTime.Now)
                {
                    ModelState.AddModelError("End_Date", "End Date must be in the future.");
                    ViewBag.Languages = GetLanguageList();
                    return View(project);
                }

                if (project.Bid_Date == default)
                {
                    ModelState.AddModelError("Bid_Date", "Bid Date is required.");
                    ViewBag.Languages = GetLanguageList();
                    return View(project);
                }

                project.UserId = userId.Value;
                project.Status = "Open";

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Project created successfully!";
                return RedirectToAction("ViewAllProjects");
            }

            ViewBag.Languages = GetLanguageList();
            return View(project);
        }

        // GET: All Projects for Logged-in Employer
        public async Task<IActionResult> ViewAllProjects()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return View(projects);
        }

        // GET: View All Bids for a Project
        [HttpGet]
        public async Task<IActionResult> ViewBids(int projectId)
        {
            if (projectId <= 0)
                return NotFound("Invalid project ID.");

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                TempData["NoBids"] = "Project not found.";
                return RedirectToAction("ViewAllProjects");
            }

            var bids = await _context.Bids
                .Include(b => b.User)
                .Include(b => b.Project)
                .Where(b => b.Project_ID == projectId)
                .ToListAsync();

            if (bids == null || !bids.Any())
            {
                TempData["NoBids"] = "This project has not received any bids yet.";
                return RedirectToAction("ViewAllProjects");
            }

            ViewBag.ProjectTitle = project.Title;
            return View(bids);
        }

        // POST: Approve Bid for Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBid(int bidId)
        {
            var bid = await _context.Bids
                .Include(b => b.User)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(b => b.Bid_ID == bidId);

            if (bid == null)
                return NotFound("Bid not found.");

            bid.Assign = "Yes";

            // Set all other bids to not assigned
            var otherBids = await _context.Bids
                .Where(b => b.Project_ID == bid.Project_ID && b.Bid_ID != bidId)
                .ToListAsync();

            foreach (var other in otherBids)
                other.Assign = "No";

            if (bid.Project != null)
            {
                bid.Project.Status = "Taken";
                bid.Project.TakenByFreelancerId = bid.UserId;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Bid by {bid.User?.FullName ?? "Freelancer"} approved.";
            return RedirectToAction("ViewBids", new { projectId = bid.Project_ID });
        }
    }
}
