
namespace BlazorApp.Client.Services {
    public class APIService: APIServiceAbstract {
        public APIService(IHttpClientFactory factory, ILogger<APIService> logger) {
            _httpClient = factory.CreateClient("api");
            _logger = logger;
        }
    }
}
