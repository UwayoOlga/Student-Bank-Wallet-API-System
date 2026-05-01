using StudentWalletAPI.DTOs;
using StudentWalletAPI.Models;

namespace StudentWalletAPI.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<StudentProfileResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<string>> ValidateStudentAsync(string studentId);
        Task UnlockAccountAsync(string studentId);
    }
}