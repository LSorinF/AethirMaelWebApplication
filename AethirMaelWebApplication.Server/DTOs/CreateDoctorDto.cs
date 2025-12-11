using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.DTOs
{
    public class CreateDoctorDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; } // Parola pentru cont
        [Required]
        public string Phone { get; set; }
        [Required]
        public int SpecializationId { get; set; } // Trebuie sa alegem specializarea
    }
}