namespace UserWallet.DTOs
{
    public class DepositDTO
    {
        public decimal Amount { get; set; }
        public string? Address { get; set; }
        public string? CardNumber { get; set; }
        public string? CardholderName { get; set; }

        public DepositDTO(decimal amount, string? address, string? cardholderName, string? cardNumber)
        {
            Amount = amount;
            CardNumber = cardNumber;
            CardholderName = cardholderName;
            Address = address;
        }

        public DepositDTO() { }
    }
}
