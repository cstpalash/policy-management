using System;
namespace policy.management.web.Models
{
    public class ComplianceItem
    {
        public string ControlId { get; set; }
        public string Provider { get; set; }
        public string ResourceType { get; set; }

        public double PreProvisionCompliant { get; set; }
        public double PreProvisionNonCompliant { get; set; }
        public double PreProvisionTotal { get { return PreProvisionCompliant + PreProvisionNonCompliant; } }
        public double PreProvisionCompliancePercentage
        {
            get {
                if (PreProvisionTotal == 0) return 0;
                return Math.Round((PreProvisionCompliant / PreProvisionTotal) * 100, 2);
            }
        }

        public double ContinuousCompliant { get; set; }
        public double ContinuousNonCompliant { get; set; }
        public double ContinuousTotal { get { return ContinuousCompliant + ContinuousNonCompliant; } }
        public double ContinuousCompliancePercentage
        {
            get
            {
                if (ContinuousTotal == 0) return 0;
                return Math.Round((ContinuousCompliant / ContinuousTotal) * 100, 2);
            }
        }
    }

    public class PolicyExecutionItem
    {
        public string EventID { get; set; }
        public string EventName { get; set; }
        public string ExecutionType { get; set; }
        public string PolicyId { get; set; }
        public string Account { get; set; }
        public bool Valid { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public string Region { get; set; }
        public string Resource { get; set; }

        public DateTime EventTime
        {
            get
            {
                return DateTime.Parse(Time);
            }
        }
    }
}
