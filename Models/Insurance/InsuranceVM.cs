using System.Runtime.CompilerServices;

namespace MVCWebApplication.Models.Insurance
{

    public class InsurancePageVM
    {
        public InsuranceFilterVM InsuranceFilter { get; set; }
        public List<InsuranceDetailVM> InsuranceDetails { get; set; } = new List<InsuranceDetailVM>();
    }
    public class InsuranceDetailVM
    {
        public long ClaimNumber { get; set; }
        public long LineNumber { get; set; }
        public string Member { get; set; }
        public string Provider { get; set; }
        public string ProcedureCode { get; set; }
        public string NetAmount { get; set; }
        public string ClaimReceivedOn { get; set; }
        public string Matching { get; set; }
        public DateTime? ServiceFromDateTime { get; set; }
        public DateTime? ServiceToDateTime { get; set; }
        public string Modifier1 { get; set; }
        public string Modifier2 { get; set; }
        public string Modifier3 { get; set; }
        public string Modifier4 { get; set; }
        public double? ContractedAmount { get; set; }
        public string PlaceOfService { get; set; }
    }

    public class InsuranceFilterVM
    {
        public long? ClaimNumber { get; set; }
        public long? LineNumber { get; set; }
        public List<DuplicateParamVM> DuplicateParams { get; set; } = new List<DuplicateParamVM>();
        public List<string> DuplicateCheck { get; set; } = new List<string>();
        public byte? MatchingCount { get; set; }
    }

    public class DuplicateParamVM
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }
}
