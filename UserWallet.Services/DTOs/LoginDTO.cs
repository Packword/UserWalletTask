namespace UserWallet.DTOs
{
    public class LoginDTO
    {
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Username { get; set; } = null!;
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; } = null!;
    }
}
