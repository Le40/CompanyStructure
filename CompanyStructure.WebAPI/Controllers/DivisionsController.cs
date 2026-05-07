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
        private readonly IDivisionService _service;
        public DivisionsController(
            IDivisionService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> GetAll(int companyId)
        {
            var result = await _service.GetAllAsync(companyId);
            return result.ToActionResult(this);
        }

        [HttpPost("/api/companies/{companyId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int companyId)
        {
            var result = await _service.CreateAsync(dto, companyId);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
    }
}
