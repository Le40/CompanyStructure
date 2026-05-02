using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class DepartmentsController : OrganisationNodeController<Department>
    {
        public DepartmentsController(IOrganisationNodeService<Department> service) : base(service)
        {
        }
    }
}
