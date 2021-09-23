using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using policy.management.web.Service;

namespace policy.management.web.Controllers
{
    public class PolicyController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPolicyService _policyService;

        public PolicyController(ILogger<HomeController> logger, IPolicyService policyService)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (policyService == null) throw new ArgumentNullException("policyService");

            _logger = logger;
            _policyService = policyService;

        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(string controlId)
        {
            var model = await _policyService.GetPolicy(controlId);
            return View(model);
        }
    }
}
