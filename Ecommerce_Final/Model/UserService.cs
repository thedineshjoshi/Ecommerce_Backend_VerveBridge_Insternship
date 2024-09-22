using Newtonsoft.Json;

namespace Ecommerce_Final.Model
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CustomerDto>> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync("api/UserRegistration/Customers");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CustomerDto>>(jsonString);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var response = await _httpClient.DeleteAsync($"api/UserRegistration/{userId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
