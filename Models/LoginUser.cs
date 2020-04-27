using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    public class LoginUser
    {
        [EmailAddress]
        [Required(ErrorMessage="Email is required")]
        public string Email {get; set;}

        [DataType(DataType.Password)]
        [Required(ErrorMessage="Enter your password")]
        public string Password { get; set;}
    }
}