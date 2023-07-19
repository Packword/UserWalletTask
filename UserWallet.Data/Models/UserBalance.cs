using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserWallet.Models
{
    public class UserBalance
    {
        public int UserId { get; set; }
        public User? User { get; set; } = null!;
        public string CurrencyId { get; set; } = null!;
        public Currency? Currency { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
