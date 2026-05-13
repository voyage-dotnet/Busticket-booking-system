using System.Text.Json;
using BusTicketSystem.MVC.ApiResponseWrapper;

namespace BusTicketSystem.MVC.Services
{
    public class VoyageApiClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public VoyageApiClient(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _client = httpClientFactory.CreateClient("VoyageAPI");
            _baseUrl = config.GetValue<string>("BaseUrl") ?? "";
        }

        public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync($"{_baseUrl}/{endpoint}");
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object data)
        {
            var response = await _client.PostAsJsonAsync($"{_baseUrl}/{endpoint}", data);
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>?> PatchAsync<T>(string endpoint, object data)
        {
            var response = await _client.PatchAsJsonAsync($"{_baseUrl}/{endpoint}", data);
            return await HandleResponse<T>(response);
        }

        private async Task<ApiResponse<T>?> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Message = "Unauthorized access. Please log in again."
                };
            }

            try
            {
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result != null)
                {
                    result.StatusCode = (int)response.StatusCode;
                    return result;
                }
            }
            catch
            {
                // If it's not our standard ApiResponse, try to extract error messages from raw JSON
                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    Message = $"API Error: {content}"
                };
            }

            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = (int)response.StatusCode,
                Message = "An unknown error occurred."
            };
        }
    }
}
