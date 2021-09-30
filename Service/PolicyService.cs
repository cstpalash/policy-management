using System;
using System.Collections.Generic;
using System.Linq;
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

        Task<List<ComplianceItem>> GetComplianceReport(string controlId);
        Task<List<ChaosEngineeringItem>> GetChaosEngineeringReport(string policyId, string source);
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

        public async Task<List<ComplianceItem>> GetComplianceReport(string controlId)
        {
            var report = new List<ComplianceItem>();
            var executionItems = new List<PolicyExecutionItem>();

            var requestBody = JsonConvert.SerializeObject(new
            {
                api = "getComplianceReport",
                payload = new {
                    controlId = controlId
                }
            });

            var stringContent = new StringContent(requestBody, UnicodeEncoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(_serviceEndpoint, stringContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                executionItems = JsonConvert.DeserializeObject<List<PolicyExecutionItem>>(apiResponse);
            }

            var map = new Dictionary<string, Dictionary<string, List<PolicyExecutionItem>>>();

            foreach(var exItem in executionItems)
            {
                var key = string.Format("{0}##{1}", exItem.PolicyId, exItem.Source);

                if (!map.ContainsKey(key))
                    map.Add(key, new Dictionary<string, List<PolicyExecutionItem>>());

                if (!map[key].ContainsKey(exItem.Resource))
                    map[key].Add(exItem.Resource, new List<PolicyExecutionItem>());

                map[key][exItem.Resource].Add(exItem);
            }

            foreach(var key in map.Keys)
            {
                var keySegments = key.Split("##", StringSplitOptions.RemoveEmptyEntries);
                var ci = new ComplianceItem { ControlId = keySegments[0] };

                switch (keySegments[1])
                {
                    case "aws.s3":
                        ci.Provider = "AWS";
                        ci.ResourceType = "S3 Bucket";
                        break;
                    case "gcp.storage":
                        ci.Provider = "GCP";
                        ci.ResourceType = "Object Storage";
                        break;
                    case "azure.blob":
                        ci.Provider = "AZURE";
                        ci.ResourceType = "Blob Storage";
                        break;
                    default:
                        break;
                }

                foreach(var resourceKey in map[key].Keys)
                {
                    //Don't count deleted resource

                    var isDeleted = map[key][resourceKey].Any(item => item.EventName == "DeleteBucket");
                    if (isDeleted) continue;

                    var preProvisionItems = map[key][resourceKey].Where(item => item.ExecutionType == "Pre Provision").OrderByDescending(item => item.EventTime).ToList();

                    if(preProvisionItems.Count > 0)
                    {
                        if (preProvisionItems[0].Valid) ci.PreProvisionCompliant++;
                        else ci.PreProvisionNonCompliant++;
                    }

                    var continuousComplianceItems = map[key][resourceKey].Where(item => item.ExecutionType == "Continuous Compliance").OrderByDescending(item => item.EventTime).ToList();

                    if (continuousComplianceItems.Count > 0)
                    {
                        if (continuousComplianceItems[0].Valid) ci.ContinuousCompliant++;
                        else ci.ContinuousNonCompliant++;
                    }
                }

                report.Add(ci);
            }

            report.Add(new ComplianceItem { ControlId = controlId, Provider = "GCP", ResourceType = "Object Storage" });
            report.Add(new ComplianceItem { ControlId = controlId, Provider = "AZURE", ResourceType = "Blob Storage" });

            return report;
        }

        public async Task<List<ChaosEngineeringItem>> GetChaosEngineeringReport(string policyId, string source)
        {
            var policies = new List<ChaosEngineeringItem>();

            var requestBody = JsonConvert.SerializeObject(new { api = "getChaosEngineeringReport", payload = new { policyId = policyId, source = source } });
            var stringContent = new StringContent(requestBody, UnicodeEncoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(_serviceEndpoint, stringContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                policies = JsonConvert.DeserializeObject<List<ChaosEngineeringItem>>(apiResponse);
            }

            return policies;
        }
    }
}
