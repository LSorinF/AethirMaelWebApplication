using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.DTOs
{
    // Completare fisa
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

        public int? AppointmentId { get; set; } //Optional daca are de-a face cu vizita
    }

    // Ce vede Pacientul/AI-ul când citește istoricul
    public class MedicalRecordDto
    {
        public int MedicalRecordId { get; set; }
        public DateTime DateCreated { get; set; }

        public string DoctorName { get; set; } // Numele doctorului care a creat fisa
        public string Specialization { get; set; }

        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string InvestigationResults { get; set; }
    }

}