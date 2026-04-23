using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AethirMaelWebApplication.Server.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Requested"; // Initial status

        [MaxLength(500)]
        public string? PatientNotes { get; set; } // Details given by the patient/chatbot

        public bool ReminderSent { get; set; } = false;

        public bool ReminderTwoHoursSent { get; set; } = false;

        // Foreign Keys (FK)
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Navigation properties:
        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; }

    }
}
