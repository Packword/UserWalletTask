using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserWallet.Models
{
    [Table("deposits")]
    public class Deposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("userId")]
        public int UserId { get; set; }
        public User? User { get; set; }
        [MaxLength(30)]
        [Column("currencyId")]
        public string CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [MaxLength(16)]
        [Column("cardNumber")]
        public string? CardNumber { get; set; }
        [MaxLength(16)]
        [Column("cardholderName")]
        public string? CardholderName { get; set; }
        [MaxLength(16)]
        [Column("address")]
        public string? Address { get; set; }
        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; }
    }
}
