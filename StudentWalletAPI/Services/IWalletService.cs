using StudentWalletAPI.DTOs;
using StudentWalletAPI.Models;

namespace StudentWalletAPI.Services
{
    public interface IWalletService
    {
        Task<ApiResponse<StudentProfileResponse>> GetBalanceAsync(string studentId);
        Task<ApiResponse<TransactionResponse>> DepositAsync(string studentId, DepositRequest request);
        Task<ApiResponse<TransactionResponse>> PayForServiceAsync(string studentId, PaymentRequest request);
        Task<ApiResponse<TransactionResponse>> TransferMoneyAsync(string studentId, TransferRequest request);
        Task<ApiResponse<List<TransactionResponse>>> GetTransactionHistoryAsync(string studentId, int page = 1, int pageSize = 10);
        Task<ApiResponse<DailySummaryResponse>> GetDailySummaryAsync(DateTime date);
        Task<ApiResponse<object>> GetOverallSummaryAsync();
    }
}