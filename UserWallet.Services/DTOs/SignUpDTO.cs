namespace UserWallet.DTOs
{
    public class SignUpDTO
    {
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string? Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string? Password { get; set; }
    }
}
