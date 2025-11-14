using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AethirMaelWebApplication.Server.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } // Also used for login

        [MaxLength(15)]
        public string Phone { get; set; }

        // Foreign Key (FK) to Specialization
        public int SpecializationId { get; set; }

        // Navigation property: Specialization object
        [ForeignKey("SpecializationId")]
        public Specialization Specialization { get; set; }

        // Navigation properties:
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
