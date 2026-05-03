using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class DivisionsController : OrganisationNodeController<Division>
    {
        public DivisionsController(IOrganisationNodeService<Division> service) : base(service)
        {
        }

        [HttpGet("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> GetAll(int companyId)
        {
            var divisions = await _service.GetAllAsync(companyId);
            return Ok(divisions);
        }

        [HttpPost("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int companyId)
        {
            var result = await _service.CreateAsync(dto, companyId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
