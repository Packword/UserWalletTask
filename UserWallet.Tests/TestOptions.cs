namespace UserWallet.Tests
{
    public static class TestOptions
    {
        public static JsonSerializerOptions JSON_OPTIONS { get; set; } = new() { PropertyNameCaseInsensitive = true };
    }
}
