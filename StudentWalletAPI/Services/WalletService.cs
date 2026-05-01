using Microsoft.EntityFrameworkCore;
using StudentWalletAPI.Data;
using StudentWalletAPI.DTOs;
using StudentWalletAPI.Models;

namespace StudentWalletAPI.Services
{
    public class WalletService : IWalletService
    {
        private readonly WalletDbContext _context;

        public WalletService(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<StudentProfileResponse>> GetBalanceAsync(string studentId)
        {
            var student = await _context.Students
                .Include(s => s.Wallet)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return ApiResponse<StudentProfileResponse>.ErrorResponse("Student not found");
            }

            var profile = new StudentProfileResponse
            {
                StudentId = student.StudentId,
                WalletId = student.Wallet?.WalletId ?? string.Empty,
                Name = student.Name,
                Balance = student.Wallet?.Balance ?? 0,
                LastUpdated = student.UpdatedAt
            };

            return ApiResponse<StudentProfileResponse>.SuccessResponse(profile);
        }

        public async Task<ApiResponse<TransactionResponse>> DepositAsync(string studentId, DepositRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var student = await _context.Students
                    .Include(s => s.Wallet)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (student == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Student not found");
                }

                if (student.Wallet == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Wallet not found for student");
                }

                // Update balance
                student.Wallet.Balance += request.Amount;
                student.UpdatedAt = DateTime.UtcNow;
                student.Wallet.UpdatedAt = DateTime.UtcNow;

                // Create transaction record
                var transactionRecord = new Transaction
                {
                    WalletId = student.Wallet.WalletId,
                    Type = TransactionType.Deposit,
                    Amount = request.Amount,
                    BalanceAfter = student.Wallet.Balance,
                    Description = string.IsNullOrEmpty(request.Description) ? "Deposit" : request.Description
                };

                _context.Transactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new TransactionResponse
                {
                    TransactionId = transactionRecord.TransactionId,
                    Type = transactionRecord.Type.ToString(),
                    Amount = transactionRecord.Amount,
                    BalanceAfter = student.Wallet.Balance,
                    Description = transactionRecord.Description,
                    CreatedAt = transactionRecord.CreatedAt
                };

                return ApiResponse<TransactionResponse>.SuccessResponse(response, "Deposit successful");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<TransactionResponse>.ErrorResponse($"Deposit failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TransactionResponse>> PayForServiceAsync(string studentId, PaymentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var student = await _context.Students
                    .Include(s => s.Wallet)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (student == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Student not found");
                }

                if (student.Wallet == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Wallet not found for student");
                }

                // Check sufficient balance
                if (student.Wallet.Balance < request.Amount)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Insufficient balance");
                }

                // Update balance
                student.Wallet.Balance -= request.Amount;
                student.UpdatedAt = DateTime.UtcNow;
                student.Wallet.UpdatedAt = DateTime.UtcNow;

                // Create transaction record
                var transactionRecord = new Transaction
                {
                    WalletId = student.Wallet.WalletId,
                    Type = TransactionType.Payment,
                    Amount = request.Amount,
                    BalanceAfter = student.Wallet.Balance,
                    Description = string.IsNullOrEmpty(request.Description) 
                        ? $"Payment for {request.ServiceType}" 
                        : request.Description,
                    ServiceType = request.ServiceType
                };

                _context.Transactions.Add(transactionRecord);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new TransactionResponse
                {
                    TransactionId = transactionRecord.TransactionId,
                    Type = transactionRecord.Type.ToString(),
                    Amount = transactionRecord.Amount,
                    BalanceAfter = student.Wallet.Balance,
                    Description = transactionRecord.Description,
                    ServiceType = transactionRecord.ServiceType?.ToString(),
                    CreatedAt = transactionRecord.CreatedAt
                };

                return ApiResponse<TransactionResponse>.SuccessResponse(response, "Payment successful");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<TransactionResponse>.ErrorResponse($"Payment failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TransactionResponse>> TransferMoneyAsync(string studentId, TransferRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var sender = await _context.Students
                    .Include(s => s.Wallet)
                    .FirstOrDefaultAsync(s => s.StudentId == studentId);

                if (sender == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Sender not found");
                }

                var receiver = await _context.Students
                    .Include(s => s.Wallet)
                    .FirstOrDefaultAsync(s => s.StudentId == request.ReceiverStudentId);

                if (receiver == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Receiver not found");
                }

                if (sender.StudentId == receiver.StudentId)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Cannot transfer to yourself");
                }

                if (sender.Wallet == null || receiver.Wallet == null)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("One or both wallets not found");
                }

                // Check sufficient balance
                if (sender.Wallet.Balance < request.Amount)
                {
                    return ApiResponse<TransactionResponse>.ErrorResponse("Insufficient balance");
                }

                // Update balances
                sender.Wallet.Balance -= request.Amount;
                receiver.Wallet.Balance += request.Amount;
                sender.UpdatedAt = DateTime.UtcNow;
                receiver.UpdatedAt = DateTime.UtcNow;
                sender.Wallet.UpdatedAt = DateTime.UtcNow;
                receiver.Wallet.UpdatedAt = DateTime.UtcNow;

                // Create transaction record for sender
                var senderTransaction = new Transaction
                {
                    WalletId = sender.Wallet.WalletId,
                    Type = TransactionType.Transfer,
                    Amount = request.Amount,
                    BalanceAfter = sender.Wallet.Balance,
                    Description = string.IsNullOrEmpty(request.Description) 
                        ? $"Transfer to {receiver.Name} ({receiver.StudentId})" 
                        : request.Description,
                    ReceiverWalletId = receiver.Wallet.WalletId
                };

                // Create transaction record for receiver
                var receiverTransaction = new Transaction
                {
                    WalletId = receiver.Wallet.WalletId,
                    Type = TransactionType.Transfer,
                    Amount = request.Amount,
                    BalanceAfter = receiver.Wallet.Balance,
                    Description = string.IsNullOrEmpty(request.Description) 
                        ? $"Transfer from {sender.Name} ({sender.StudentId})" 
                        : request.Description,
                    ReceiverWalletId = sender.Wallet.WalletId
                };

                _context.Transactions.AddRange(senderTransaction, receiverTransaction);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new TransactionResponse
                {
                    TransactionId = senderTransaction.TransactionId,
                    Type = senderTransaction.Type.ToString(),
                    Amount = senderTransaction.Amount,
                    BalanceAfter = sender.Wallet.Balance,
                    Description = senderTransaction.Description,
                    ReceiverStudentId = receiver.StudentId,
                    CreatedAt = senderTransaction.CreatedAt
                };

                return ApiResponse<TransactionResponse>.SuccessResponse(response, "Transfer successful");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<TransactionResponse>.ErrorResponse($"Transfer failed: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<TransactionResponse>>> GetTransactionHistoryAsync(string studentId, int page = 1, int pageSize = 10)
        {
            var student = await _context.Students
                .Include(s => s.Wallet)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return ApiResponse<List<TransactionResponse>>.ErrorResponse("Student not found");
            }

            if (student.Wallet == null)
            {
                return ApiResponse<List<TransactionResponse>>.ErrorResponse("Wallet not found");
            }

            var transactions = await _context.Transactions
                .Where(t => t.WalletId == student.Wallet.WalletId)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponse
                {
                    TransactionId = t.TransactionId,
                    Type = t.Type.ToString(),
                    Amount = t.Type == TransactionType.Payment 
                        ? -t.Amount 
                        : (t.Type == TransactionType.Transfer && t.Description.Contains("Transfer to")) 
                            ? -t.Amount 
                            : t.Amount,
                    BalanceAfter = t.BalanceAfter,
                    Description = t.Description,
                    ServiceType = t.ServiceType.HasValue ? t.ServiceType.ToString() : null,
                    ReceiverStudentId = t.ReceiverWallet != null ? t.ReceiverWallet.StudentId : null,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return ApiResponse<List<TransactionResponse>>.SuccessResponse(transactions);
        }

        public async Task<ApiResponse<DailySummaryResponse>> GetDailySummaryAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var transactions = await _context.Transactions
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt < endDate)
                .ToListAsync();

            var summary = new DailySummaryResponse
            {
                Date = date.Date,
                TotalTransactions = transactions.Count,
                TotalDeposits = transactions.Where(t => t.Type == TransactionType.Deposit).Sum(t => t.Amount),
                TotalPayments = transactions.Where(t => t.Type == TransactionType.Payment).Sum(t => t.Amount),
                TotalTransfers = transactions.Where(t => t.Type == TransactionType.Transfer).Sum(t => t.Amount) / 2, // Divide by 2 because transfers are recorded twice
            };

            summary.NetAmount = summary.TotalDeposits - summary.TotalPayments;

            return ApiResponse<DailySummaryResponse>.SuccessResponse(summary);
        }

        public async Task<ApiResponse<object>> GetOverallSummaryAsync()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalBalance = await _context.Wallets.SumAsync(w => w.Balance);
            
            var totalDeposits = await _context.Transactions
                .Where(t => t.Type == TransactionType.Deposit)
                .SumAsync(t => t.Amount);
            
            var totalPayments = await _context.Transactions
                .Where(t => t.Type == TransactionType.Payment)
                .SumAsync(t => t.Amount);
            
            var totalTransfers = await _context.Transactions
                .Where(t => t.Type == TransactionType.Transfer)
                .SumAsync(t => t.Amount) / 2; // Divide by 2 because transfers are recorded twice

            var summary = new
            {
                TotalStudents = totalStudents,
                TotalSystemBalance = totalBalance,
                TotalDeposits = totalDeposits,
                TotalPayments = totalPayments,
                TotalTransfers = totalTransfers,
                NetTransactionAmount = totalDeposits - totalPayments
            };

            return ApiResponse<object>.SuccessResponse(summary);
        }
    }
}