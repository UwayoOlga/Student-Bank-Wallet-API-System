using Microsoft.AspNetCore.Mvc;
using StudentWalletAPI.DTOs;
using StudentWalletAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace StudentWalletAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Student login with Student ID and PIN
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<StudentProfileResponse>>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse<StudentProfileResponse>.ErrorResponse("Invalid input", errors));
            }

            var result = await _authService.LoginAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Validate if a student exists (for transfers)
        /// </summary>
        [HttpGet("validate/{studentId}")]
        public async Task<ActionResult<ApiResponse<string>>> ValidateStudent(string studentId)
        {
            try
            {
                var result = await _authService.ValidateStudentAsync(studentId);
                
                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse($"Internal server error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Unlock a student account (admin function)
        /// </summary>
        [HttpPost("unlock/{studentId}")]
        public async Task<ActionResult<ApiResponse<string>>> UnlockAccount(string studentId)
        {
            await _authService.UnlockAccountAsync(studentId);
            return Ok(ApiResponse<string>.SuccessResponse("Account unlocked successfully"));
        }
    }
}