using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace UserWallet.Tests.Extensions
{
    public static class HttpResponseExtension
    {
        public async static Task<T?> GetContentAsync<T>(this HttpResponseMessage response)
            => await response.Content.ReadFromJsonAsync<T>(WalletJsonOptions.JSON_SERIALIZER_OPTIONS);

        public static IDictionary<string, string[]>? GetErrors(this HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync();
            try
            {
                var errors = JsonSerializer.Deserialize<ValidationProblemDetails>(content.Result);
                return errors?.Errors;
            }
            catch
            {
                return new Dictionary<string, string[]>() { { "default", new string[] { content.Result } } };
            }
        }

        private class ValidationProblemDetails
        {
            [JsonPropertyName("status")]
            public int? Status { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("traceId")]
            public string TraceId { get; set; }

            [JsonPropertyName("errors")]
            public IDictionary<string, string[]> Errors { get; set; }
        }
    }
}
