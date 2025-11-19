using ERP_Models.Entities.Common.Responses;
using System.Net.Http.Json;
using System.Text.Json;

namespace ERP_Clients.Services.Base
{
    public abstract class BaseHttpClient
    {
        protected readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected BaseHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object request)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, request);
            return await DeserializeResponse<T>(response);
        }

        protected async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await DeserializeResponse<T>(response);
        }

        protected async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object request)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, request);
            return await DeserializeResponse<T>(response);
        }

        protected async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return await DeserializeResponse<T>(response);
        }

        private static async Task<ApiResponse<T>> DeserializeResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions);
                return data ?? ApiResponse<T>.Fail("Empty response");
            }

            var error = await response.Content.ReadAsStringAsync();
            return ApiResponse<T>.Fail($"HTTP {response.StatusCode}: {error}");
        }
    }
}