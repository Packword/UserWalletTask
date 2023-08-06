namespace UserWallet.Tests
{
    public static class TestData
    {
        public const string DEFAULT_USER_USERNAME = "maxim";
        public const string DEFAULT_USER_PASSWORD = "123456";
        public const int DEFAULT_USER_ID = 2;
        public const string ADMIN_USERNAME = "Admin";
        public const string ADMIN_PASSWORD = "1234";
        public const int ADMIN_ID = 1;
        public const int DEFAULT_USERS_COUNT = 2;
        public const string CRYPTO_CURRENCY_ID = "btc";
        public const string CRYPTO_ADDRESS = "1234567890123456";
        public const string FIAT_CURRENCY_ID = "rub";
        public const string FIAT_CARDHOLDER = "1234567890123456";
        public const string FIAT_CARDNUMBER = "1234567890123456";
        public const decimal DEFAULT_DEPOSIT_AMOUNT = 50;
        public static JsonSerializerOptions JSON_OPTIONS = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
}
