using System.Text;
using System.Text.Json;
using BusTicketSystem.MVC.ApiResponseWrapper;

namespace BusTicketSystem.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static StringContent ToJsonContent(object body)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
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
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                if (result != null)
                {
                    result.StatusCode = (int)response.StatusCode;
                    return result;
                }
            }
            catch (JsonException)
            {
                return new ApiResponse<T>
                {
                    Success = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Message = response.IsSuccessStatusCode ? "Success" : $"API Error: {content}",
                    Data = response.IsSuccessStatusCode && !string.IsNullOrEmpty(content) ? JsonSerializer.Deserialize<T>(content, _jsonOptions)! : default!
                };
            }

            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = (int)response.StatusCode,
                Message = "An unknown error occurred."
            };
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await ProcessResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object body)
        {
            var response = await _httpClient.PostAsync(endpoint, ToJsonContent(body));
            return await ProcessResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object body)
        {
            var response = await _httpClient.PutAsync(endpoint, ToJsonContent(body));
            return await ProcessResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object body)
        {
            var response = await _httpClient.PatchAsync(endpoint, ToJsonContent(body));
            return await ProcessResponse<T>(response);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return await ProcessResponse<T>(response);
        }
    }
}
