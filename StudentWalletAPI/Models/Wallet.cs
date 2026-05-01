using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentWalletAPI.Models
{
    public class Wallet
    {
        [Key]
        public string WalletId { get; set; } = string.Empty;

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
        
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}