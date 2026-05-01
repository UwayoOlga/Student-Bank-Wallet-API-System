using System.ComponentModel.DataAnnotations;
using StudentWalletAPI.Models;

namespace StudentWalletAPI.DTOs
{
    public class DepositRequest
    {
        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }

    public class PaymentRequest
    {
        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }

        [Required]
        public ServiceType ServiceType { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }

    public class TransferRequest
    {
        [Required]
        [StringLength(20)]
        public string ReceiverStudentId { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}