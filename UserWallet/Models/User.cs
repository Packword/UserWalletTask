using System.ComponentModel.DataAnnotations;

namespace UserWallet.Models
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(8, MinimumLength = 4)]
        public string? Username { get; set; }

        [StringLength(8, MinimumLength = 4)]
        public string? Password { get; set; }

        [StringRange(AllowableValues = new[] {"Admin", "User"})]
        public string? Role { get; set; }
        public Dictionary<string, decimal> Balances { get; set; } = new Dictionary<string, decimal>();

        public class StringRangeAttribute : ValidationAttribute
        {
            public string[] AllowableValues { get; set; }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (AllowableValues?.Contains(value?.ToString()) == true)
                {
                    return ValidationResult.Success;
                }

                var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
                return new ValidationResult(msg);
            }
        }
    }
}
