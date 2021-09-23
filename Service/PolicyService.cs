using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using policy.management.web.Models;

namespace policy.management.web.Service
{
    public interface IPolicyService
    {
        Task<List<GlobalPolicy>> GetAllPolicies();
        Task<GlobalPolicy> GetPolicy(string controlId);
    }

    public class PolicyService : IPolicyService
    {
        private readonly ILogger<PolicyService> _logger;
        private readonly HttpClient _httpClient;

        private readonly string _serviceEndpoint;

        public PolicyService(ILogger<PolicyService> logger, HttpClient httpClient)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (httpClient == null) throw new ArgumentNullException("httpClient");

            _serviceEndpoint = "https://pd3zcnq4qa.execute-api.us-west-2.amazonaws.com/default/fn-policy-management";
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<List<GlobalPolicy>> GetAllPolicies()
        {
            var policies = new List<GlobalPolicy>();

            var requestBody = JsonConvert.SerializeObject(new { api = "getAllPolicies" });
            var stringContent = new StringContent(requestBody, UnicodeEncoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(_serviceEndpoint, stringContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                policies = JsonConvert.DeserializeObject<List<GlobalPolicy>>(apiResponse);
            }

            return policies;
        }

        public async Task<GlobalPolicy> GetPolicy(string controlId)
        {
            GlobalPolicy item = null;
            var requestBody = JsonConvert.SerializeObject(new
            {
                api = "getPolicy",
                payload = new {
                    controlId = controlId
                }
            });

            var stringContent = new StringContent(requestBody, UnicodeEncoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(_serviceEndpoint, stringContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                item = JsonConvert.DeserializeObject<GlobalPolicy>(apiResponse);
            }

            return item;
        }
    }
}
