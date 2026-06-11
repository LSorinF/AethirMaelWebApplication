using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.DTOs
{
    public class CreateMedicalRecordDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public string Symptoms { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        [Required]
        public string Treatment { get; set; }

        public string? InvestigationResults { get; set; }

        public int? AppointmentId { get; set; } 
    }
}