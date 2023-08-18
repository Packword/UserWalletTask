namespace UserWallet.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }

        public LoginDTO() { }
        public LoginDTO(string? username, string? password)
        {
            Username = username;
            Password = password;
        }
    }
}
