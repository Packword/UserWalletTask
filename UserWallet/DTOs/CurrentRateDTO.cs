namespace UserWallet.DTOs
{
    public class CurrentRateDTO
    {
        public DateTime DateTime { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}
