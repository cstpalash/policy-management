using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using policy.management.web.Models;
using policy.management.web.Service;

namespace policy.management.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPolicyService _policyService;

        public HomeController(ILogger<HomeController> logger, IPolicyService policyService)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (policyService == null) throw new ArgumentNullException("policyService");

            _logger = logger;
            _policyService = policyService;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllPolicies()
        {
            var allPolicies = await _policyService.GetAllPolicies();
            return Json(allPolicies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
