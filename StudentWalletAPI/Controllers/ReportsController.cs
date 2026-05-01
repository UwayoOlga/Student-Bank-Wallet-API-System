using Microsoft.AspNetCore.Mvc;
using StudentWalletAPI.DTOs;
using StudentWalletAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace StudentWalletAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public ReportsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Get daily transaction summary for a specific date
        /// </summary>
        [HttpGet("daily/{date}")]
        public async Task<ActionResult<ApiResponse<DailySummaryResponse>>> GetDailySummary(DateTime date)
        {
            var result = await _walletService.GetDailySummaryAsync(date);
            return Ok(result);
        }

        /// <summary>
        /// Get daily transaction summary for today
        /// </summary>
        [HttpGet("daily")]
        public async Task<ActionResult<ApiResponse<DailySummaryResponse>>> GetTodaysSummary()
        {
            var result = await _walletService.GetDailySummaryAsync(DateTime.Today);
            return Ok(result);
        }

        /// <summary>
        /// Get overall system summary (total deposits vs payments)
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<object>>> GetOverallSummary()
        {
            var result = await _walletService.GetOverallSummaryAsync();
            return Ok(result);
        }
    }
}