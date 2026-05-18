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
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{6,}$",
            ErrorMessage = "Parola trebuie să conțină minim 6 caractere, o literă mare, un număr și un caracter special.")]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public int SpecializationId { get; set; } 
    }
}