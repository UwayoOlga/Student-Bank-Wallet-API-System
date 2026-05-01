namespace StudentWalletAPI.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class StudentProfileResponse
    {
        public string StudentId { get; set; } = string.Empty;
        public string WalletId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Token { get; set; }
    }

    public class TransactionResponse
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ServiceType { get; set; }
        public string? ReceiverWalletId { get; set; }
        public string? ReceiverStudentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DailySummaryResponse
    {
        public DateTime Date { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal TotalTransfers { get; set; }
        public decimal NetAmount { get; set; }
    }
}