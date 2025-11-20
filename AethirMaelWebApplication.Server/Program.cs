using Microsoft.EntityFrameworkCore;
using AethirMaelWebApplication.Server.Data;
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

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();