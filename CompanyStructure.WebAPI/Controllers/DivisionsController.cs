using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class DivisionsController : OrganisationNodeController<Division>
    {
        private readonly IDivisionService _divisionService;
        public DivisionsController(
            IOrganisationNodeService<Division> service,
            IDivisionService divisionService) : base(service)
        {
            _divisionService = divisionService;
        }

        [HttpGet("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> GetAll(int companyId)
        {
            var divisions = await _divisionService.GetAllAsync(companyId);
            return Ok(divisions);
        }

        [HttpPost("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int companyId)
        {
            var result = await _divisionService.CreateAsync(dto, companyId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
