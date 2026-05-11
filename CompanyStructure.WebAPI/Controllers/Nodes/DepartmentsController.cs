using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Nodes
{
    public class DepartmentsController : NodeController<Department>
    {
        private readonly IDepartmentService _service;
        public DepartmentsController(
            IDepartmentService service) : base(service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("/api/Projects/{projectId}/[controller]")]
        public async Task<IActionResult> GetAll([FromRoute] int projectId, [FromQuery] PaginationQuery pagination)
        {
            var result = await _service.GetAllAsync(projectId, pagination);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/api/Projects/{projectId}/[controller]")]
        public async Task<IActionResult> Create(CreateNodeRequest dto, int projectId)
        {
            var result = await _service.CreateAsync(dto, projectId);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
    }
}
