using BlazorApp.Shared.Models;

namespace BlazorApp.Client.Services {
    public interface IAPIService {
        public Task<ServiceTag> GetServiceByIP(string ip);
        public Task<DNSLookup> GetDNSLookup(string fqdn);
    }
}
