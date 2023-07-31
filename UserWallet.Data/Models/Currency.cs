using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserWallet.Data.Enums;

namespace UserWallet.Models
{
    [Table("currencies")]
    public class Currency
    {
        [Key]
        [MaxLength(30)]
        [Column("id")]
        public string Id { get; set; } = null!;

        [Column("is_available")]
        public bool IsAvailable { get; set; }

        [Column("type")]
        public CurrencyType Type { get; set; }
    }
}
