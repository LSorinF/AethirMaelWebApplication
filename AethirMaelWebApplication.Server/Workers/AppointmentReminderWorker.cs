using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace AethirMaelWebApplication.Server.Workers
{
    public class AppointmentReminderWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        public AppointmentReminderWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                        var now = DateTime.Now;

                        // ---------------------------------------------------------
                        // CAZUL 1: Reminder de 24 ORE (Ziua următoare)
                        // ---------------------------------------------------------
                        var tomorrow = now.AddDays(1);
                        var window24hStart = now.AddHours(23); // Cautam intre 23h si 25h de acum

                        var appointments24h = await context.Appointments
                            .Include(a => a.Patient).Include(a => a.Doctor)
                            .Where(a => a.Status == "Confirmată"
                                     && !a.ReminderSent // Nu a fost trimis cel de 24h
                                     && a.AppointmentDate > now
                                     && a.AppointmentDate <= tomorrow)
                            .ToListAsync(stoppingToken);

                        foreach (var app in appointments24h)
                        {
                            if (!string.IsNullOrEmpty(app.Patient.Email))
                            {
                                var subject = "📅 Reminder: Programare Mâine - AethirMael Clinic";
                                var body = $@"
                                    <h3>Salut {app.Patient.FirstName},</h3>
                                    <p>Îți reamintim că ai o programare mâine, <b>{app.AppointmentDate:dd/MM/yyyy}</b> la ora <b>{app.AppointmentDate:HH:mm}</b>.</p>
                                    <p>Medic: Dr. {app.Doctor.LastName} {app.Doctor.FirstName}</p>";

                                await emailService.SendEmailAsync(app.Patient.Email, subject, body);
                                app.ReminderSent = true; // Marcam primul reminder
                            }
                        }

                        // ---------------------------------------------------------
                        // CAZUL 2: Reminder de 2 ORE (Urgent)
                        // ---------------------------------------------------------
                        var twoHoursFromNow = now.AddHours(2);

                        var appointments2h = await context.Appointments
                            .Include(a => a.Patient).Include(a => a.Doctor)
                            .Where(a => a.Status == "Confirmată"
                                     && !a.ReminderTwoHoursSent // Nu a fost trimis cel de 2h
                                     && a.AppointmentDate > now
                                     && a.AppointmentDate <= twoHoursFromNow) // E in urmatoarele 2 ore
                            .ToListAsync(stoppingToken);

                        foreach (var app in appointments2h)
                        {
                            if (!string.IsNullOrEmpty(app.Patient.Email))
                            {
                                var subject = "⏰Programare în 2 ore - AethirMael Clinic";
                                var body = $@"
                                    <h3>Salut {app.Patient.FirstName},</h3>
                                    <p>Nu uita! Ai o programare azi la ora <b>{app.AppointmentDate:HH:mm}</b> (în aproximativ 2 ore).</p>
                                    <p>Te asteptam cu drag!.</p>
                                    <p>Medic: Dr. {app.Doctor.LastName} {app.Doctor.FirstName}</p>";

                                await emailService.SendEmailAsync(app.Patient.Email, subject, body);
                                app.ReminderTwoHoursSent = true; // Marcam al doilea reminder
                            }
                        }

                        if (appointments24h.Any() || appointments2h.Any())
                        {
                            await context.SaveChangesAsync(stoppingToken);
                            Console.WriteLine($"[ReminderWorker] Trimis: {appointments24h.Count} (24h) și {appointments2h.Count} (2h).");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ReminderWorker] ❌ Eroare: {ex.Message}");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}