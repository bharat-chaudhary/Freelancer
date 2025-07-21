using Freelancer.Data;
using Freelancer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Freelancer.Controllers
{
    public class TaskController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskController> _logger;

        public TaskController(AppDbContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? projectId)
        {
            try
            {
                var tasks = _context.ProjectTasks
                    .Include(t => t.Project)
                    .Include(t => t.User)
                    .AsQueryable();

                if (projectId.HasValue)
                {
                    tasks = tasks.Where(t => t.ProjectId == projectId);
                    var project = await _context.Projects.FindAsync(projectId.Value);
                    ViewBag.ProjectTitle = project?.Title ?? "Project Tasks";
                    ViewBag.ProjectId = projectId.Value;
                }
                else
                {
                    ViewBag.ProjectTitle = "All Tasks";
                    ViewBag.ProjectId = null;
                }

                return View(await tasks.OrderByDescending(t => t.DueDate).ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tasks");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int projectId)
        {
            ViewBag.ProjectId = projectId;
            var project = await _context.Projects.FindAsync(projectId);
            ViewBag.ProjectTitle = project?.Title;
            return View(new ProjectTask { ProjectId = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectTask task)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                ModelState.AddModelError(string.Empty, "User not logged in.");
                return View(task);
            }

            task.UserId = userId.Value;

            if (!await _context.Projects.AnyAsync(p => p.ProjectId == task.ProjectId))
            {
                ModelState.AddModelError(string.Empty, "Project does not exist.");
                return View(task);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.ProjectTasks.Add(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving task");
                    ModelState.AddModelError("", "An error occurred while saving the task.");
                }
            }

            var project = await _context.Projects.FindAsync(task.ProjectId);
            ViewBag.ProjectTitle = project?.Title;
            ViewBag.ProjectId = task.ProjectId;
            return View(task);
        }
        // GET: Task/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskId == id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Freelancer"), "UserId", "FullName", task.UserId);
            return View(task);
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectTask task)
        {
            if (id != task.TaskId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { projectId = task.ProjectId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ProjectTasks.Any(e => e.TaskId == task.TaskId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["UserId"] = new SelectList(_context.Users.Where(u => u.Role == "Freelancer"), "UserId", "FullName", task.UserId);
            return View(task);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.TaskId == id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: Task/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.ProjectTasks.FindAsync(id);
            if (task != null)
            {
                int? projectId = task.ProjectId;
                _context.ProjectTasks.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { projectId });
            }

            return NotFound();
        }
    }
}
