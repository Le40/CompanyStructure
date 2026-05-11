using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Nodes
{
    public class DivisionsController : NodeController<Division>
    {
        private readonly IDivisionService _service;
        public DivisionsController(
            IDivisionService service) : base(service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("/api/Companies/{companyId}/[controller]")]
        public async Task<IActionResult> GetAll([FromRoute]int companyId, [FromQuery] PaginationQuery pagination)
        {
            var result = await _service.GetAllAsync(companyId, pagination);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/api/Companies/{companyId}/[controller]")]
        public async Task<IActionResult> Create(CreateNodeRequest dto, int companyId)
        {
            var result = await _service.CreateAsync(dto, companyId);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
    }
}
