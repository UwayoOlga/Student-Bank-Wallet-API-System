using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentWalletAPI.Models
{
    public class Student
    {
        [Key]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(4)]
        public string PIN { get; set; } = string.Empty;

        public int FailedLoginAttempts { get; set; } = 0;

        public bool IsLocked { get; set; } = false;

        public DateTime? LockedUntil { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - One student has one wallet
        public virtual Wallet? Wallet { get; set; }
    }
}