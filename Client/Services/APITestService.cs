
namespace BlazorApp.Client.Services {
    public class APITestService: APIServiceAbstract {
        public APITestService(IHttpClientFactory factory, ILogger<APIService> logger) {
            _httpClient = factory.CreateClient("dev_api");
            _logger = logger;
        }
    }
}
