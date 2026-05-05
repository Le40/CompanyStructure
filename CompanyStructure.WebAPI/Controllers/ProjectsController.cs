using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Controllers.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    public class ProjectsController : OrganisationNodeController<Project>
    {
        private readonly IProjectService _service;
        public ProjectsController(
            IProjectService service) : base(service)
        {
            _service = service;
        }
        [HttpGet("/api/divisions/{divisionId}/[controller]")]
        public async Task<IActionResult> GetAll(int divisionId)
        {
            var result = await _service.GetAllAsync(divisionId);
            return result.ToActionResult(this);
        }

        [HttpPost("/api/divisions/{divisionId}/[controller]")]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto, int divisionId)
        {
            var result = await _service.CreateAsync(dto, divisionId);
            if (!result.Success)
            {
                return result.ToActionResult(this);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }
    }
}
