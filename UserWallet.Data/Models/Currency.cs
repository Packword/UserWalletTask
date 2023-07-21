using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserWallet.Models
{
    [Table("currencies")]
    public class Currency
    {
        [Key]
        [MaxLength(30)]
        [Column("id")]
        public string Id { get; set; }
        [Column("isAvailable")]
        public bool IsAvailable { get; set; }
        [MaxLength(10)]
        [Column("type")]
        public string Type { get; set; }
    }
}
