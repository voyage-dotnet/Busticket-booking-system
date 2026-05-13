using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BusTicketSystem.MVC.Services
{
    /// <summary>
    /// A thin, reusable HTTP client wrapper that routes all API calls
    /// to the BusTicketSystem.Web backend. Automatically injects the
    /// JWT stored in session into every request that requires auth.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Private helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Attaches the JWT from session (key "JwtToken") as a Bearer header.
        /// </summary>
        private void AttachToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private static StringContent ToJsonContent(object body)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Reads the JSON body from the response and deserialises the
        /// <c>data</c> property of the standard ApiResponse wrapper:
        /// <code>{ "data": {...}, "message": "...", "statusCode": 200 }</code>
        /// </summary>
        private static async Task<T?> ReadDataAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json)) return default;

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("data", out var dataProp))
            {
                return JsonSerializer.Deserialize<T>(dataProp.GetRawText(), _jsonOptions);
            }
            // If there's no wrapper, try deserialising the root directly
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Public HTTP verbs
        // ─────────────────────────────────────────────────────────────────────

        public async Task<T?> GetAsync<T>(string endpoint, bool requiresAuth = true)
        {
            if (requiresAuth) AttachToken();
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await ReadDataAsync<T>(response);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object body, bool requiresAuth = true)
        {
            if (requiresAuth) AttachToken();
            var response = await _httpClient.PostAsync(endpoint, ToJsonContent(body));
            response.EnsureSuccessStatusCode();
            return await ReadDataAsync<T>(response);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object body, bool requiresAuth = true)
        {
            if (requiresAuth) AttachToken();
            var response = await _httpClient.PutAsync(endpoint, ToJsonContent(body));
            response.EnsureSuccessStatusCode();
            return await ReadDataAsync<T>(response);
        }

        /// <summary>
        /// Returns true if the API call succeeded (2xx).
        /// Used when the response body is irrelevant.
        /// </summary>
        public async Task<bool> PostVoidAsync(string endpoint, object body, bool requiresAuth = true)
        {
            if (requiresAuth) AttachToken();
            var response = await _httpClient.PostAsync(endpoint, ToJsonContent(body));
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PutVoidAsync(string endpoint, object body, bool requiresAuth = true)
        {
            if (requiresAuth) AttachToken();
            var response = await _httpClient.PutAsync(endpoint, ToJsonContent(body));
            return response.IsSuccessStatusCode;
        }
    }
}
