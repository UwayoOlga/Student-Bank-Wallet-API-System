using Microsoft.EntityFrameworkCore;
using StudentWalletAPI.Data;
using StudentWalletAPI.DTOs;
using StudentWalletAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace StudentWalletAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly WalletDbContext _context;
        private readonly IConfiguration _configuration;
        private const int MaxFailedAttempts = 3;
        private const int LockoutMinutes = 30;

        public AuthService(WalletDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<StudentProfileResponse>> LoginAsync(LoginRequest request)
        {
            var student = await _context.Students
                .Include(s => s.Wallet)
                .FirstOrDefaultAsync(s => s.StudentId == request.StudentId);

            if (student == null)
            {
                return ApiResponse<StudentProfileResponse>.ErrorResponse("Invalid Student ID or PIN");
            }

            // Check if account is locked
            if (student.IsLocked)
            {
                if (student.LockedUntil.HasValue && student.LockedUntil > DateTime.UtcNow)
                {
                    var remainingTime = student.LockedUntil.Value - DateTime.UtcNow;
                    return ApiResponse<StudentProfileResponse>.ErrorResponse(
                        $"Account is locked. Try again in {remainingTime.Minutes} minutes and {remainingTime.Seconds} seconds.");
                }
                else
                {
                    // Auto-unlock if lockout period has expired
                    await UnlockAccountAsync(student.StudentId);
                    student.IsLocked = false;
                    student.LockedUntil = null;
                    student.FailedLoginAttempts = 0;
                }
            }

            // Validate PIN
            if (student.PIN != request.PIN)
            {
                student.FailedLoginAttempts++;
                
                if (student.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    student.IsLocked = true;
                    student.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                    await _context.SaveChangesAsync();
                    
                    return ApiResponse<StudentProfileResponse>.ErrorResponse(
                        $"Account locked due to {MaxFailedAttempts} failed login attempts. Try again in {LockoutMinutes} minutes.");
                }

                await _context.SaveChangesAsync();
                
                var remainingAttempts = MaxFailedAttempts - student.FailedLoginAttempts;
                return ApiResponse<StudentProfileResponse>.ErrorResponse(
                    $"Invalid PIN. {remainingAttempts} attempts remaining before account lockout.");
            }

            // Successful login - reset failed attempts
            student.FailedLoginAttempts = 0;
            student.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var profile = new StudentProfileResponse
            {
                StudentId = student.StudentId,
                WalletId = student.Wallet?.WalletId ?? string.Empty,
                Name = student.Name,
                Balance = student.Wallet?.Balance ?? 0,
                LastUpdated = student.UpdatedAt,
                Token = GenerateJwtToken(student)
            };

            return ApiResponse<StudentProfileResponse>.SuccessResponse(profile, "Login successful");
        }

        public async Task<ApiResponse<string>> ValidateStudentAsync(string studentId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return ApiResponse<string>.ErrorResponse("Student not found");
            }

            if (student.IsLocked)
            {
                return ApiResponse<string>.ErrorResponse("Student account is locked");
            }

            return ApiResponse<string>.SuccessResponse(student.Name, "Student validated");
        }

        public async Task UnlockAccountAsync(string studentId)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student != null)
            {
                student.IsLocked = false;
                student.LockedUntil = null;
                student.FailedLoginAttempts = 0;
                student.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
 
        private string GenerateJwtToken(Student student)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "SecretKeyForStudentWalletAPI2024");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, student.StudentId),
                    new Claim(ClaimTypes.Name, student.Name)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}