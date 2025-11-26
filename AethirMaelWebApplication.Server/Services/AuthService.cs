using AethirMaelWebApplication.Server.Data;
using AethirMaelWebApplication.Server.DTOs;
using AethirMaelWebApplication.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Necesar pentru Token
using System.IdentityModel.Tokens.Jwt; // Necesar pentru JWT
using System.Security.Claims; // Necesar pentru Claims (Datele din token)
using System.Security.Cryptography;
using System.Text;

namespace AethirMaelWebApplication.Server.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // Avem nevoie de config pentru a citi cheia

        // Injectam si IConfiguration
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Patient)
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null) return null;

            var inputHash = HashPassword(loginDto.Password);
            if (user.PasswordHash != inputHash) return null;

            // GENERARE TOKEN REAL
            string token = CreateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Role = user.Role,
                Name = user.Patient?.FirstName ?? user.Doctor?.FirstName ?? "Admin"
            };
        }

        private string CreateToken(User user)
        {
            // 1. Definim Claim-urile (informatiile din buletinul digital)
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Daca e pacient sau doctor, adaugam si ID-ul specific
            if (user.PatientId.HasValue)
                claims.Add(new Claim("PatientId", user.PatientId.Value.ToString()));

            if (user.DoctorId.HasValue)
                claims.Add(new Claim("DoctorId", user.DoctorId.Value.ToString()));

            // 2. Luam cheia secreta din appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtSettings:Key").Value!));

            // 3. Semnam token-ul
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Cream token-ul
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token valabil 1 zi
                signingCredentials: creds,
                issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
                audience: _configuration.GetSection("JwtSettings:Audience").Value
            );

            // 5. Il scriem ca string
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}