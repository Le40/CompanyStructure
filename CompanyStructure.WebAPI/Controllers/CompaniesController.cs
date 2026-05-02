using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CompaniesController : OrganisationNodeController<Company>
    {
        public CompaniesController(IOrganisationNodeService<Company> service) : base(service)
        {
        }
    }
}
