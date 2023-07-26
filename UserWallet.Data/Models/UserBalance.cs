using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserWallet.Models
{
    [Table("userBalances")]
    public class UserBalance
    {
        [Column("user_id")]
        public int UserId { get; set; }

        public User? User { get; set; } = null!;

        [MaxLength(30)]
        [Column("currency_id")]
        public string CurrencyId { get; set; } = null!;

        public Currency? Currency { get; set; } = null!;

        [Column("amount")]
        public decimal Amount { get; set; }
    }
}
