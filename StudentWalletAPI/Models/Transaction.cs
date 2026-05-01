using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentWalletAPI.Models
{
    public enum TransactionType
    {
        Deposit,
        Payment,
        Transfer
    }

    public enum ServiceType
    {
        Cafeteria,
        Printing,
        Transport,
        Other
    }

    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string WalletId { get; set; } = string.Empty;

        [Required]
        public TransactionType Type { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public ServiceType? ServiceType { get; set; }

        // For transfers - nullable for deposits/payments
        public string? ReceiverWalletId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; } = null!;

        [ForeignKey("ReceiverWalletId")]
        public virtual Wallet? ReceiverWallet { get; set; }
    }
}