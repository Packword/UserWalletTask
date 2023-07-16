using System.ComponentModel.DataAnnotations;

namespace UserWallet.DTOs
{
    public class DepositDTO
    {
        [Range(0.1, 100)]
        [Required]
        public decimal Amount { get; set; }
        [StringLength(16, MinimumLength = 16)]
        public string? Card_number { get; set; }
        [StringLength(16, MinimumLength = 2)]
        public string? Cardholder_name { get; set; }
        [StringLength(16, MinimumLength = 16)]
        public string? Address { get; set; }
    }
}
