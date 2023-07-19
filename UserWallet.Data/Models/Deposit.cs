using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserWallet.Models
{
    public class Deposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        public decimal Amount { get; set; }
        public string? CardNumber { get; set; }
        public string? CardholderName { get; set; }
        public string? Address { get; set; }
        public string Status { get; set; }
    }
}
