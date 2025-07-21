using System.ComponentModel.DataAnnotations;

namespace Freelancer.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        // Optional: if you want to use this for login roles
        public string? Role { get; set; }
    }
}
