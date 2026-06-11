using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.Hubs;
using AethirMaelWebApplication.Server.Services;
using AethirMaelWebApplication.Server.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --Cors config --
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClientPolicy",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:4200",  // Portul standard Angular CLI
                "https://localhost:4200",
                "https://localhost:57997" // Port actual
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
        });
});


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<MedicalRecordService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<EmailService>(); 
builder.Services.AddHostedService<AppointmentReminderWorker>(); 
builder.Services.AddScoped<MaelAiService>();

builder.Services.AddSignalR();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("JwtSettings:Key").Value!)),
        ValidateIssuer = false, 
        ValidateAudience = false 
    };
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await SeedData.EnsurePopulated(app.Services);

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AngularClientPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notificationHub");

app.MapFallbackToFile("/index.html");

app.Run();