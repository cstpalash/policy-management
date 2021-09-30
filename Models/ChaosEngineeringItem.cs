using System;
namespace policy.management.web.Models
{
    public class ChaosEngineeringItem
    {
        public string PolicyId { get; set; }
        public string Time { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
        public bool Valid { get; set; }
    }
}
