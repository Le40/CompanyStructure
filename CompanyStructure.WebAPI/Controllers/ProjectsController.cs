using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class ProjectsController : OrganisationNodeController<Project>
    {
        private readonly IProjectService _projectService;
        public ProjectsController(
            IOrganisationNodeService<Project> service,
            IProjectService projectService) : base(service)
        {
            _projectService = projectService;
        }
        [HttpGet("/api/divisions/{divisionId}/[controller]")]
        public async Task<IActionResult> GetAll(int divisionId)
        {
            var projects = await _projectService.GetAllAsync(divisionId);
            return Ok(projects);
        }

        [HttpPost("/api/divisions/{divisionId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int divisionId)
        {
            var result = await _projectService.CreateAsync(dto, divisionId);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
