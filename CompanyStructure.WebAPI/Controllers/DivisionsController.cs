using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class DivisionsController : OrganisationNodeController<Division>
    {
        public DivisionsController(IOrganisationNodeService<Division> service) : base(service)
        {
        }
    }
}
