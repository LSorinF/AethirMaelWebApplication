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

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            //Verifica daca emailul exista deja
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return false;

            var Patient = new Patient
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                Phone = registerDto.Phone,           
                CNP = registerDto.CNP,               
                DateOfBirth = registerDto.DateOfBirth 
            };

            await _context.Patients.AddAsync(Patient);
            await _context.SaveChangesAsync();

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                Role = "Patient", // Rol implicit
                PatientId = Patient.PatientId 
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            if (user.PatientId.HasValue)
                claims.Add(new Claim("PatientId", user.PatientId.Value.ToString()));

            if (user.DoctorId.HasValue)
                claims.Add(new Claim("DoctorId", user.DoctorId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtSettings:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Cream token-ul
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token valabil 1 zi
                signingCredentials: creds,
                issuer: _configuration.GetSection("JwtSettings:Issuer").Value,
                audience: _configuration.GetSection("JwtSettings:Audience").Value
            );
            //Il scriem ca string
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