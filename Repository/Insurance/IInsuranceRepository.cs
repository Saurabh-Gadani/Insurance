using MVCWebApplication.Models.Insurance;

namespace MVCWebApplication.Repository.Insurance
{
    public interface IInsuranceRepository
    {
        Task<InsurancePageVM> GetInsuranceDetails(InsuranceFilterVM filter);
    }
}
