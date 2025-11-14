using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        // CNP (Personal Numeric Code) must be unique
        [Required]
        [MaxLength(13)]
        public string CNP { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        // Navigation properties:
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
