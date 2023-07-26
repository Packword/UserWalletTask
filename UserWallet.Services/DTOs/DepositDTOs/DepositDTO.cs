using System.ComponentModel.DataAnnotations;

namespace UserWallet.DTOs
{
    public class DepositDTO
    {
        [Range(0.1, 100)]
        [Required]
        public decimal Amount { get; set; }
        public string? CardNumber { get; set; }
        public string? CardholderName { get; set; }
        public string? Address { get; set; }
    }
}
