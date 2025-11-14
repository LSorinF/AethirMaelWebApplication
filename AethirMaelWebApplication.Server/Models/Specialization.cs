using System.ComponentModel.DataAnnotations;

namespace AethirMaelWebApplication.Server.Models
{
    public class Specialization
    {
        [Key]
        public int SpecializationId { get; set; }

        // Name of the specialization (e.g., Cardiology, Dermatology)
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Navigation property: Access the list of Doctors associated with this specialization
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
