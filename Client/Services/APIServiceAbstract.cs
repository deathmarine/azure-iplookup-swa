
using BlazorApp.Shared.Models;
using System.Text.Json;

namespace BlazorApp.Client.Services {
    public class APIServiceAbstract: IAPIService {
        public HttpClient _httpClient;
        public ILogger _logger;
        public async Task<DNSLookup> GetDNSLookup(string fqdn) {
            var response = await JsonSerializer.DeserializeAsync<DNSLookup>(
                await _httpClient.GetStreamAsync($"api/dnslookup?fqdn={fqdn}"),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return response;
        }

        public async Task<ServiceTag> GetServiceByIP(string ip) {
            var response = await JsonSerializer.DeserializeAsync<ServiceTag>(
                await _httpClient.GetStreamAsync($"api/ipcheck?ip={ip}"),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            /***
            var stream = await _httpClient.GetStreamAsync($"api/ipcheck?ip={ip}");
            using (var reader = new StreamReader(stream)) {
                return await reader.ReadToEndAsync();
            }***/
            return response;
        }
    }
}
