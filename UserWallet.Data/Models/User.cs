using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserWallet.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [StringLength(8, MinimumLength = 4)]
        [Column("username")]
        public string Username { get; set; } = null!;

        [StringLength(8, MinimumLength = 4)]
        [Column("password")]
        public string Password { get; set; } = null!;

        [MaxLength(30)]
        [Column("role")]
        public string Role { get; set; } = null!;

        [Column("is_blocked")]
        public bool IsBlocked { get; set; } = false;

        public ICollection<UserBalance>? Balances { get; set; }

        public ICollection<Deposit>? Deposits { get; set; }
    }
}
