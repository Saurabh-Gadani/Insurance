using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVCWebApplication.Models.Insurance;
using MVCWebApplication.Repository.Insurance;

namespace MVCWebApplication.Controllers
{
    public class InsuranceController : Controller
    {
        public IInsuranceRepository _repository;

        public InsuranceController(IInsuranceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repository.GetInsuranceDetails(new InsuranceFilterVM());
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SearchInsurance([FromForm]InsurancePageVM filterVM)
        {
            var data = await _repository.GetInsuranceDetails(filterVM.InsuranceFilter);
            return View("Index", data);
        }
    }
}
