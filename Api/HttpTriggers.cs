using System.Net;
using System.Text.Json;
using BlazorApp.Shared;
using BlazorApp.Shared.Models;
using DnsClient;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class HttpTriggers
    {
        private readonly ILogger _logger;
        string matchip = "(\\b25[0-5]|\\b2[0-4][0-9]|\\b[01]?[0-9][0-9]?)(\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}";
        string matchfqdn = "([a-zA-Z0-9]+(-[a-zA-Z0-9]+)*\\.)+[a-zA-Z]{2,}";
        private readonly LookupClient _lookup;

        public HttpTriggers(ILoggerFactory loggerFactory, LookupClient lookupClient)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggers>();
            _lookup = lookupClient;
        }

        [Function("ipcheck")]
        public async Task<HttpResponseData> RunIPCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req) {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string ip = req.Query["ip"];
            if (ip == null || ip.Length < 1) {
                var res1 = req.CreateResponse(HttpStatusCode.BadRequest);
                await res1.WriteStringAsync("Please pass an IP on the query string");
                return res1;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(ip, matchip)) {
                var res2 = req.CreateResponse(HttpStatusCode.BadRequest);
                await res2.WriteStringAsync("Please pass a valid IP on the query string");
                return res2;
            }
            var res = req.CreateResponse(HttpStatusCode.OK);
            var service = JsonSerializer.Serialize(findServiceByIP(ip));
            res.WriteString(service);
            //string responseMessage = JsonSerializer.Serialize(service);
            return res;
        }

        [Function("dnslookup")]
        public async Task<HttpResponseData> RunDNSLookup([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req) {
            string fqdn = req.Query["fqdn"];
            if (fqdn == null || fqdn.Length < 1) {
                var res1 = req.CreateResponse(HttpStatusCode.BadRequest);
                await res1.WriteStringAsync("Please pass a domain name on the query string");
                return res1;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(fqdn, matchfqdn)) {
                var res2 = req.CreateResponse(HttpStatusCode.BadRequest);
                await res2.WriteStringAsync("Please pass a valid name on the query string");
                return res2;
            }
            _logger.LogInformation($"Checking name: {fqdn}");
            var payload = await Dns.GetHostEntryAsync(fqdn);
            String[] ips = new string[payload.AddressList.Length];
            for (int i = 0; i < payload.AddressList.Length; i++) {
                ips[i] = payload.AddressList[i].ToString();
            }

            var query = await _lookup.QueryAsync(fqdn, QueryType.ANY);
            var _records = query.AllRecords;
            var doc = new {
                HostName = payload.HostName,
                AddressList = ips,
                Records = _records

            };
            string responseMessage = JsonSerializer.Serialize(doc);
            _logger.LogInformation(responseMessage);
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteStringAsync(responseMessage);
            return res;
        }

        public ServiceTag findServiceByIP(string ip_address) {
            string dat = File.ReadAllText("./ServiceTags_Public_20250217.json"); //validate location
            Cloud _var = JsonSerializer.Deserialize<Cloud>(dat);
            _logger.LogInformation($"Searching for IP = {ip_address} in Azure");
            if (ip_address.Contains(':') && !ip_address.Contains("::"))
                ip_address = ip_address.Substring(0, ip_address.LastIndexOf(":"));
            var address = IPAddress.Parse(ip_address);
            int s_count = 0, i_count = 0;
            foreach (var service_tag in _var.values) {
                foreach (var ip_range in service_tag.properties.addressPrefixes) {
                    var network = IPNetwork.Parse(ip_range);
                    if (network.Contains(address)) {
                        _logger.LogInformation($"Found IP: {ip_address} for {service_tag.name}");
                        return service_tag;
                    }
                    i_count++;
                }
                s_count++;
            }
            _logger.LogInformation($"IP Not Found: Services {s_count}, IPs {i_count}");
            return new ServiceTag {
                id = "none",
                name = "Not Found"
            };
        }

    }
}
