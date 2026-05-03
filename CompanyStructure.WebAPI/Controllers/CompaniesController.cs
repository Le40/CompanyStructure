using CompanyStructure.Application.Services;
using CompanyStructure.Application.DTOs.OrganisationNodes;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _service.GetAllAsync();
            return Ok(companies);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
