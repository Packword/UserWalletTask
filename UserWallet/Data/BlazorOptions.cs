using System.Text.Json;

namespace UserWallet.Data
{
    public static class BlazorOptions
    {
        public static readonly JsonSerializerOptions JSON_SERIALIZE_OPTIONS = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
}
