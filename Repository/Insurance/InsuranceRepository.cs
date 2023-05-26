using Dapper;
using MVCWebApplication.Models.Insurance;
using MVCWebApplication.Repository.Helper;
using System.Data;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace MVCWebApplication.Repository.Insurance
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly DbContext _context;

        public InsuranceRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<InsurancePageVM> GetInsuranceDetails(InsuranceFilterVM filter)
        {
            if (filter.DuplicateParams.Count == 0)
            {
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Member", Value = "Member" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Provider", Value = "Provider" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Service From Date Time", Value = "ServiceFromDateTime" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Service To Date Time", Value = "ServiceToDateTime" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Procedure Code", Value = "ProcedureCode" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Modifier 1", Value = "Modifier1" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Modifier 2", Value = "Modifier2" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Modifier 3", Value = "Modifier3" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Modifier 4", Value = "Modifier4" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Contracted Amount", Value = "ContractedAmount" });
                filter.DuplicateParams.Add(new DuplicateParamVM { Key = "Place Of Service", Value = "PlaceOfService" });
            }

            filter.DuplicateCheck = filter.DuplicateParams.Where(x => x.Selected).Select(x=>x.Value).ToList();

            InsurancePageVM InsurancePageVM = new InsurancePageVM()
            {
                InsuranceFilter = filter
            };

            try
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ClaimNumber", filter.ClaimNumber);
                parameters.Add("LineNumber", filter.LineNumber);

                var query = @"SELECT pc.ClaimNumber,s.LineNumber,s.ProcedureCode,pc.Member,pc.Provider,pc.NetAmount,pc.ClaimReceivedOn,
                            s.ServiceFromDateTime,s.ServiceToDateTime,s.Modifier1,s.Modifier2,s.Modifier3,s.Modifier4,pc.PlaceOfService,
                            pc.ContractedAmount
                            FROM ProfessionalClaims pc
                            LEFT JOIN Services s ON s.ClaimNumber=pc.ClaimNumber
                            WHERE (@ClaimNumber IS NULL OR @ClaimNumber='' OR pc.ClaimNumber=@ClaimNumber) AND
                            (@LineNumber IS NULL OR @LineNumber='' OR s.LineNumber=@LineNumber)";

                using var con = _context.CreateConnection();
                InsurancePageVM.InsuranceDetails = (await con.QueryAsync<InsuranceDetailVM>(query, parameters)).ToList();

                if (filter.DuplicateCheck?.Count > 0 && filter.MatchingCount.HasValue)
                {
                    List<InsuranceDetailVM> duplicateMatch = new List<InsuranceDetailVM>();
                    var combination = filter.DuplicateCheck.DifferentCombinations(filter.MatchingCount.Value).ToList();

                    foreach (var cmb in combination)
                    {
                        var groupByData = InsurancePageVM.InsuranceDetails.GroupBy(x => new
                        {
                            Name = cmb.Contains("Member") ? x.Member : null,
                            Provider = cmb.Contains("Provider") ? x.Provider : null,
                            ServiceFromDateTime = cmb.Contains("ServiceFromDateTime") ? x.ServiceFromDateTime : null,
                            ServiceToDateTime = cmb.Contains("ServiceToDateTime") ? x.ServiceToDateTime : null,
                            ProcedureCode = cmb.Contains("ProcedureCode") ? x.ProcedureCode : null,
                            Modifier1 = cmb.Contains("Modifier1") ? x.Modifier1 : null,
                            Modifier2 = cmb.Contains("Modifier2") ? x.Modifier2 : null,
                            Modifier3 = cmb.Contains("Modifier3") ? x.Modifier3 : null,
                            Modifier4 = cmb.Contains("Modifier4") ? x.Modifier4 : null,
                            ContractedAmount = cmb.Contains("ContractedAmount") ? x.ContractedAmount : null,
                            PlaceOfService = cmb.Contains("PlaceOfService") ? x.PlaceOfService : null,
                        },
                        (key, g) => new
                        {
                            Count = g.Count(),
                            Data = g.ToList()
                        }).Where(x => x.Count > 1).Select(x => x.Data);

                        groupByData.ToList().ForEach(x => duplicateMatch.AddRange(x));

                        foreach (var gData in groupByData.ToList())
                        {
                            gData.ForEach(x => x.Matching = string.Join("-", cmb));
                            duplicateMatch.AddRange(gData);
                        }
                    }

                    InsurancePageVM.InsuranceDetails = duplicateMatch.Distinct().ToList();
                }
            }
            catch (Exception e)
            {

            }
            return InsurancePageVM;
        }
    }
}
