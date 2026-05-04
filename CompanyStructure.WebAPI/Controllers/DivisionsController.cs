using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Controllers.Helpers;
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
            var result = await _divisionService.GetAllAsync(companyId);
            return result.ToActionResult(this);
        }

        [HttpPost("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int companyId)
        {
            var result = await _divisionService.CreateAsync(dto, companyId);
            if (!result.Success)
            {
                return result.ToActionResult(this);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
