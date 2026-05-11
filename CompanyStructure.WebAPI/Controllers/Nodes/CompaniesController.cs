using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Nodes
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController : ControllerBase//: OrganisationNodeController<Company>
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationQuery pagination)
        {
            var result = await _service.GetAllAsync(pagination);
            return result.ToActionResult(this);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpGet("{id}/structure")]
        public async Task<IActionResult> GetStructureById(int id)
        {
            var result = await _service.GetStructureByIdAsync(id);
            return result.ToActionResult(this);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyRequest dto)
        {
            var result = await _service.CreateAsync(dto);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateNodeRequest dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
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
