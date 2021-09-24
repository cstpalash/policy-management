using System;
namespace policy.management.web.Models
{
    public class GlobalPolicy
    {
        public string ControlId { get; set; }
        public string Category { get; set; }
        public string Standard { get; set; }
        public string Owner { get; set; }
        public string Level { get; set; }
        public string Requirement { get; set; }
        public string Mechanism { get; set; }
        public bool GoldProd { get; set; }
        public string Repo { get; set; }
    }
}
