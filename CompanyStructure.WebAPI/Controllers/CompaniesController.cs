using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.WebAPI.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase//: OrganisationNodeController<Company>
    {
        private readonly ICompanyService _service;
        private readonly ILogger<CompaniesController> _logger;
        public CompaniesController(ICompanyService service, ILogger<CompaniesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return result.ToActionResult(this);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrganisationNodeDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result.Success)
                return result.ToActionResult(this);

            return NoContent();
        }
    }
}
