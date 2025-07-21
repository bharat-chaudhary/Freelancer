using Microsoft.AspNetCore.Http;

namespace Freelancer.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string FreelancerName { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }

        public IFormFile ReceiptImage { get; set; } // For uploaded file
    }
}
