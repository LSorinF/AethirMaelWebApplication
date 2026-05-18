using AethirMaelWebApplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AethirMaelWebApplication.Server.Services
{
    public class MaelAiService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MaelAiService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> AskMaelAsync(string userMessage, ClaimsPrincipal currentUser)
        {
            try
            {
                // 1. Extragem ID-ul din Token
                var claim = currentUser.FindFirst(ClaimTypes.NameIdentifier) ?? currentUser.FindFirst("id");
                if (claim == null)
                    return "🤖 Eroare internă: Nu am putut găsi ID-ul tău în token-ul de securitate.";

                int userId = int.Parse(claim.Value);

                // 2. Validare Cheie API (Revenim la Groq)
                var apiKey = _configuration["Groq:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("LIPESTE_AICI") || apiKey == "string")
                {
                    return "🤖 Oops! Conexiunea mea cu serverele Groq este oprită. Verifică appsettings.json.";
                }

                // 3. STATISTICI GLOBALE CLINICĂ
                int totalPatients = await _context.Patients.CountAsync();
                int totalAppointments = await _context.Appointments.CountAsync();
                int totalRecords = await _context.MedicalRecords.CountAsync();
                int totalDoctors = await _context.Doctors.CountAsync();

                var doctorsList = await _context.Doctors
                    .Include(d => d.Specialization)
                    .Select(d => $"Dr. {d.LastName} {d.FirstName} ({d.Specialization.Name})")
                    .ToListAsync();
                string doctorsStr = string.Join(", ", doctorsList);

                // 4. IDENTIFICARE UTILIZATOR ȘI EXTRAGERE DATE RELEVANTE
                var user = await _context.Users
                    .Include(u => u.Patient)
                    .Include(u => u.Doctor)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                string userRole = "Vizitator";
                string userName = "Utilizator Necunoscut";
                string dataContext = "Eroare: Nu s-au putut încărca datele.";

                if (user != null)
                {
                    // PACIENT
                    if (user.Patient != null)
                    {
                        userRole = "Pacient";
                        userName = $"{user.Patient.LastName} {user.Patient.FirstName}";

                        var records = await _context.MedicalRecords
                            .Where(r => r.PatientId == user.Patient.PatientId)
                            .OrderByDescending(r => r.DateCreated)
                            .Select(r => $"[Data: {r.DateCreated:dd/MM/yyyy}] Diagnostic: {r.Diagnosis} | Simptome: {r.Symptoms} | Tratament: {r.Treatment}")
                            .ToListAsync();

                        if (records.Any())
                            dataContext = "Acesta este dosarul TĂU medical personal:\n" + string.Join("\n", records);
                        else
                            dataContext = "Ești pacient, dar momentan nu ai nicio fișă medicală înregistrată.";
                    }
                    // MEDIC
                    else if (user.Doctor != null)
                    {
                        userRole = "Doctor";
                        userName = $"Dr. {user.Doctor.LastName} {user.Doctor.FirstName}";

                        var doctorPatients = await _context.MedicalRecords
                            .Include(r => r.Patient)
                            .Where(r => r.DoctorId == user.Doctor.DoctorId)
                            .OrderByDescending(r => r.DateCreated)
                            .Take(30)
                            .Select(r => $"Pacient: {r.Patient.LastName} {r.Patient.FirstName} | Data: {r.DateCreated:dd/MM/yyyy} | Diagnostic: {r.Diagnosis} | Simptome: {r.Symptoms} | Tratament: {r.Treatment}")
                            .ToListAsync();

                        if (doctorPatients.Any())
                            dataContext = "Ai acces la fișele pacienților tăi:\n" + string.Join("\n", doctorPatients);
                        else
                            dataContext = "Nu ai completat încă nicio fișă medicală pentru vreun pacient.";
                    }
                    // ADMINISTRATOR
                    else if (!string.IsNullOrEmpty(user.Role) && user.Role.ToLower().Contains("admin"))
                    {
                        userRole = "Administrator";
                        userName = "Administrator";

                        var allRecentRecords = await _context.MedicalRecords
                            .Include(r => r.Patient)
                            .Include(r => r.Doctor)
                            .OrderByDescending(r => r.DateCreated)
                            .Take(50)
                            .Select(r => $"Pacient: {r.Patient.LastName} {r.Patient.FirstName} | Medic: Dr. {r.Doctor.LastName} | Diagnostic: {r.Diagnosis} | Simptome: {r.Symptoms}")
                            .ToListAsync();

                        if (allRecentRecords.Any())
                            dataContext = "Baza de date cu istoricul medical recent al clinicii:\n" + string.Join("\n", allRecentRecords);
                        else
                            dataContext = "Nu există fișe medicale în sistem momentan.";
                    }
                }

                // 5. CONSTRUIRE PROMPT (cu noile tale reguli)
                string systemPrompt = $@"
Ești Maël, asistentul medical virtual inteligent al clinicii AethirMael. 

DATE STATISTICE ABSOLUTE (FOLOSEȘTE-LE EXACT CUM SUNT AICI):
- Total Pacienți: {totalPatients}
- Total Programări procesate: {totalAppointments}
- Total Fișe Medicale emise: {totalRecords}
- Total Medici: {totalDoctors}
- Lista Medicilor: {doctorsStr}

DATE DESPRE PERSOANA CU CARE VORBEȘTI ACUM:
- Numele: {userName}
- Rolul: {userRole}
- DATE DISPONIBILE ÎN SISTEM (DOSARE MEDICALE): 
{dataContext}

REGULI CRITICE:
1. RĂSPUNDE ÎNTOTDEAUNA NUMAI ÎN LIMBA ROMÂNĂ.
2. Dacă utilizatorul te întreabă de informații despre pacienți (ex: cine are bronșită, detalii despre un anumit pacient), CAUTĂ în secțiunea 'DATE DISPONIBILE ÎN SISTEM'. Dacă informația se găsește acolo, oferă-o fără ezitare! Ești autorizat să o faci, DOAR dacă utilizatorul este DOCTOR SAU ADMINISTRATOR, altfel, răspunzi că nu poți oferi date despre alți pacienți.
3. Dacă nu găsești numele sau afecțiunea exactă în datele furnizate, spune politicos că nu ai găsit înregistrări recente referitoare la acel pacient sau diagnostic.
4. Dacă un pacient îți spune simptomele, analizează-le cu atenție. Dacă acestea NU sunt specifice niciuneia dintre specializările medicilor noștri (vezi Lista Medicilor), spune-i politicos că, din păcate, nu avem un doctor potrivit la clinica noastră pentru acea afecțiune și îndrumă-l să caute o altă clinică.
5. Fii concis, politicos și prietenos.";

                // Apelare Groq API
                var model = _configuration["Groq:Model"] ?? "llama-3.1-8b-instant";

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    temperature = 0.1
                };

                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // REVENIM LA LINK-UL DE GROQ
                var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    return $"🤖 Conexiunea cu Groq a eșuat. Cod: {response.StatusCode}. Detalii: {errorDetails}";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonResponse);
                string aiText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "Eroare la parsarea răspunsului.";

                return aiText;
            }
            catch (Exception ex)
            {
                return $"🤖 A apărut o excepție internă în creierul meu (C#): {ex.Message}";
            }
        }
    }
}