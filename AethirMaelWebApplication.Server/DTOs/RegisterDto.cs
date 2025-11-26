using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        // CNP, Telefon etc.
    }
}
