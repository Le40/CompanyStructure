using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class DepartmentsController : OrganisationNodeController<Department>
    {
        private readonly IDepartmentService _service;
        public DepartmentsController(
            IDepartmentService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("/api/projects/{projectId}/[controller]")]
        public async Task<IActionResult> GetAll(int projectId)
        {
            var result = await _service.GetAllAsync(projectId);
            return result.ToActionResult(this);
        }
            
        [HttpPost("/api/projects/{projectId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int projectId)
        {
            var result = await _service.CreateAsync(dto, projectId);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
    }
}
