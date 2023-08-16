namespace UserWallet.DTOs
{
    public class ChangeUserPasswordDTO
    {
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string? NewPassword { get; set; }

        [Required]
        public string? OldPassword { get; set; }
    }
}
