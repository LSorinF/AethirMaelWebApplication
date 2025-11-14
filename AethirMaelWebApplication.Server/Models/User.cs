using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        // Will store the password hash, not clear text!
        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } // E.g., 'patient', 'doctor', 'admin'

        // Optional Relationships:
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }

        // Navigation properties (optional)
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }
    }
}
