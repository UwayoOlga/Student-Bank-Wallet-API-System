using System.ComponentModel.DataAnnotations;

namespace StudentWalletAPI.DTOs
{
    public class LoginRequest
    {
        [Required]
        [StringLength(20)]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [StringLength(4)]
        public string PIN { get; set; } = string.Empty;
    }
}