using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.DTOs
{
    public class UpdateMedicalRecordDto
    {
        [Required]
        public int MedicalRecordId { get; set; }

        [Required]
        public string Symptoms { get; set; }

        [Required]
        public string Diagnosis { get; set; }

        [Required]
        public string Treatment { get; set; }

        public string? InvestigationResults { get; set; }
    }
}
