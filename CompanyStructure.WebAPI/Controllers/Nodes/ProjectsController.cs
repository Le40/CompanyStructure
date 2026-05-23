using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Nodes
{
    public class ProjectsController : NodeController<Project>
    {
        private readonly IProjectService _service;
        public ProjectsController(
            IProjectService service) : base(service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("/api/Divisions/{divisionId:int}/[controller]")]
        public async Task<IActionResult> GetAll([FromRoute] int divisionId, [FromQuery] PaginationQuery pagination)
        {
            var result = await _service.GetAllAsync(divisionId, pagination);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/api/Divisions/{divisionId:int}/[controller]")]
        public async Task<IActionResult> Create(CreateNodeRequest dto, int divisionId)
        {
            var result = await _service.CreateAsync(dto, divisionId);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
    }
}
