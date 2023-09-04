namespace UserWallet.OptionsModels
{
    public class WalletJsonOptions
    {
        public static readonly JsonSerializerOptions JSON_SERIALIZER_OPTIONS = new() { PropertyNameCaseInsensitive = true };
    }
}
