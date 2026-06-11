namespace AethirMaelWebApplication.Server.DTOs
{
    public class AdminDashboardStatsDto
    {
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public List<MonthlyStatDto> AppointmentsPerMonth { get; set; }
        public List<MonthlyStatDto> PatientsPerMonth { get; set; }
    }
}
