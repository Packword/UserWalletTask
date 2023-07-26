using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserWallet.Data.Enums;

namespace UserWallet.Models
{
    [Table("deposits")]
    public class Deposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        public User? User { get; set; }

        [MaxLength(30)]
        [Column("currency_id")]
        public string CurrencyId { get; set; } = null!;

        public Currency? Currency { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("additional_data", TypeName = "jsonb")]
        public string AdditionalData { get; set; } = null!;

        [Column("status")]
        public DepositStatuses Status { get; set; }
    }
}
