using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class DepartmentsController : OrganisationNodeController<Department>
    {
        private readonly IDepartmentService _departmentsService;
        public DepartmentsController(
            IOrganisationNodeService<Department> service,
            IDepartmentService departmentService) : base(service)
        {
            _departmentsService = departmentService;
        }

        [HttpGet("/api/projects/{projectId}/[controller]")]
        public async Task<IActionResult> GetAll(int projectId)
        {
            var departments = await _departmentsService.GetAllAsync(projectId);
            return Ok(departments);
        }
            
        [HttpPost("/api/projects/{projectId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int projectId)
        {
            var result = await _departmentsService.CreateAsync(dto, projectId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
