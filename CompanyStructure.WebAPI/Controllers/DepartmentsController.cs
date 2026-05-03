using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    //[Route("api/[controller]")]
    public class DepartmentsController : OrganisationNodeController<Department>
    {
        public DepartmentsController(IOrganisationNodeService<Department> service) : base(service)
        {
        }
        [HttpGet("/api/projects/{projectId}/[controller]")]
        public async Task<IActionResult> GetAll(int projectId)
        {
            var departments = await _service.GetAllAsync(projectId);
            return Ok(departments);
        }
            
        [HttpPost("/api/projects/{projectId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int projectId)
        {
            var result = await _service.CreateAsync(dto, projectId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
