using System.ComponentModel.DataAnnotations;

namespace UserWallet.DTOs
{
    public class UserDTO
    {
        [StringLength(8, MinimumLength = 4)]
        public string Username { get; set; }
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}
