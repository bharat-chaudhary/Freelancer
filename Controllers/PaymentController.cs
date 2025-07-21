using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Freelancer.Models;
using Freelancer.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Freelancer.Data;

namespace Freelancer.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string _receiptImagePath;

        public PaymentController(AppDbContext context)
        {
            _context = context;
            _receiptImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Ensure the receipts folder exists
            if (!Directory.Exists(_receiptImagePath))
            {
                Directory.CreateDirectory(_receiptImagePath);
            }
        }

        // Step 1: Initiate Payment
        public IActionResult InitiatePayment(int? projectId)
        {
            if (!projectId.HasValue)
                return BadRequest("ProjectId is required.");

            var project = _context.Projects
                .Include(p => p.TakenByFreelancer)
                .FirstOrDefault(p => p.ProjectId == projectId.Value);

            var currentUserId = HttpContext.Session.GetInt32("UserId");

            if (project == null || currentUserId == null)
                return NotFound();

            if (project.UserId != currentUserId.Value)
                return Unauthorized();

            if (project.TakenByFreelancerId == null)
                return BadRequest("No freelancer has been assigned to this project.");

            var model = new PaymentViewModel
            {
                ProjectId = project.ProjectId,
                Title = project.Title,
                Amount = project.Budget,
                FreelancerName = project.TakenByFreelancer?.FullName ?? "Freelancer"
            };

            return View(model);
        }

        // Step 2: Process Payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel model, IFormFile ReceiptImage)
        {
            if (ModelState.IsValid)
            {
                var project = await _context.Projects
                    .Include(p => p.TakenByFreelancer)
                    .FirstOrDefaultAsync(p => p.ProjectId == model.ProjectId);

                if (project == null)
                    return NotFound();

                var currentUserId = HttpContext.Session.GetInt32("UserId");

                if (project.UserId != currentUserId)
                    return Unauthorized();

                // Handle receipt image upload
                string receiptImagePath = null;
                if (ReceiptImage != null && ReceiptImage.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ReceiptImage.FileName);
                    var filePath = Path.Combine(_receiptImagePath, fileName);

                    // Ensure the receipts directory exists (double-check here too)
                    if (!Directory.Exists(_receiptImagePath))
                    {
                        Directory.CreateDirectory(_receiptImagePath);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ReceiptImage.CopyToAsync(stream);
                    }

                    receiptImagePath = "/receipts/" + fileName; // Save the relative path
                }

                // Create a new payment record
                var payment = new Payment
                {
                    ProjectId = model.ProjectId,
                    Amount = model.Amount,
                    PayerId = currentUserId.Value,
                    PayeeId = project.TakenByFreelancerId.Value,
                    PaymentDate = DateTime.Now,
                    Status = PaymentStatus.Completed,
                    PaymentMethod = model.PaymentMethod,
                    TransactionId = model.TransactionId ?? Guid.NewGuid().ToString(),
                    ReceiptImagePath = receiptImagePath
                };

                try
                {
                    // Save payment to the database
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    // Update project status
                    project.Status = "Paid";
                    _context.Projects.Update(project);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("PaymentConfirmation", new { paymentId = payment.PaymentId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while processing the payment: " + ex.Message);
                }
            }

            return View("InitiatePayment", model);
        }

        // Step 3: Payment Confirmation
        public IActionResult PaymentConfirmation(int paymentId)
        {
            var payment = _context.Payments
                .FirstOrDefault(p => p.PaymentId == paymentId);

            if (payment == null)
                return NotFound();

            return View(payment);
        }
    }
}
