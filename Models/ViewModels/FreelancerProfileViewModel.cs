using System.ComponentModel.DataAnnotations;

namespace Freelancer.Models.ViewModels
{
    public class FreelancerProfileViewModel
    {
        public User User { get; set; }
        public Freelancers Freelancer { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
    }
}
