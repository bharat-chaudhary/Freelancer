using Freelancer.Data;
using Freelancer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Freelancer.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Admin - Dashboard
        public IActionResult Dashboard()
        {
            ViewBag.Username = "Admin";
            ViewBag.ProfileImage = "/images/contacts_16076050.png";
            return View();
        }

        // Admin - View Users
        public IActionResult Users(string search)
        {
            ViewBag.Username = "Admin";
            ViewBag.ProfileImage = "/images/contacts_16076050.png";

            var users = _context.Users
                .Where(u => (u.Role == "Freelancer" || u.Role == "Employer") &&
                            (string.IsNullOrEmpty(search) ||
                             u.Username.Contains(search) ||
                             u.Email.Contains(search) ||
                             u.Role.Contains(search)))
                .ToList();

            ViewBag.Search = search;
            return View(users);
        }

        // Admin - View Single User Details
        public IActionResult UserDetails(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();
            return View(user);
        }

        // Admin - Delete User
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Users));
        }

        // Admin - View Audit Logs
        public IActionResult AuditLogs(string search)
        {
            var query = _context.Audits
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.User.Username.Contains(search));
            }

            var auditLogs = query.OrderByDescending(a => a.Login_time).ToList();
            ViewBag.Search = search;
            return View(auditLogs);
        }

        // 🔥 Admin - View All Projects
        public async Task<IActionResult> ViewAllProjects()
        {
            var projects = await _context.Projects
                .Include(p => p.User) // To see Employer info
                .ToListAsync();

            return View(projects);
        }

        // 🔥 Admin - View Single Project Details
        public async Task<IActionResult> ProjectDetails(int id)
        {
            if (id == 0)
                return NotFound();

            var project = await _context.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            if (project == null)
                return NotFound();

            return View(project);
        }

       
    }
}
