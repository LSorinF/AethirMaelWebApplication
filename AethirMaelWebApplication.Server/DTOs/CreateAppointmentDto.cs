using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; } 

        public string? PatientNotes { get; set; } 
    }
}
