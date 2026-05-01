using Microsoft.AspNetCore.Mvc;
using StudentWalletAPI.DTOs;
using StudentWalletAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace StudentWalletAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Get student balance and profile
        /// </summary>
        [HttpGet("balance/{studentId}")]
        public async Task<ActionResult<ApiResponse<StudentProfileResponse>>> GetBalance(string studentId)
        {
            try
            {
                var result = await _walletService.GetBalanceAsync(studentId);
                
                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<StudentProfileResponse>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Deposit money to student wallet
        /// </summary>
        [HttpPost("deposit/{studentId}")]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> Deposit(string studentId, [FromBody] DepositRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<TransactionResponse>.ErrorResponse("Invalid input", errors));
            }

            var result = await _walletService.DepositAsync(studentId, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Pay for services (cafeteria, printing, transport)
        /// </summary>
        [HttpPost("pay/{studentId}")]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> PayForService(string studentId, [FromBody] PaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<TransactionResponse>.ErrorResponse("Invalid input", errors));
            }

            var result = await _walletService.PayForServiceAsync(studentId, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Transfer money to another student
        /// </summary>
        [HttpPost("transfer/{studentId}")]
        public async Task<ActionResult<ApiResponse<TransactionResponse>>> TransferMoney(string studentId, [FromBody] TransferRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    
                    return BadRequest(ApiResponse<TransactionResponse>.ErrorResponse("Invalid input", errors));
                }

                var result = await _walletService.TransferMoneyAsync(studentId, request);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<TransactionResponse>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get transaction history for a student
        /// </summary>
        [HttpGet("history/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<TransactionResponse>>>> GetTransactionHistory(
            string studentId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _walletService.GetTransactionHistoryAsync(studentId, page, pageSize);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}