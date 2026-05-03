using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class ProjectsController : OrganisationNodeController<Project>
    {
        public ProjectsController(IOrganisationNodeService<Project> service) : base(service)
        {
        }
        [HttpGet("/api/divisions/{divisionId}/[controller]")]
        public async Task<IActionResult> GetAll(int divisionId)
        {
            var projects = await _service.GetAllAsync(divisionId);
            return Ok(projects);
        }

        [HttpPost("/api/divisions/{divisionId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int divisionId)
        {
            var result = await _service.CreateAsync(dto, divisionId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
