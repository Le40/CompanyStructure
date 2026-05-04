using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Controllers.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class OrganisationNodeController<TEntity> : ControllerBase
        where TEntity : class, IOrganisationNode
    {
        protected readonly IOrganisationNodeService<TEntity> _service;

        protected OrganisationNodeController(IOrganisationNodeService<TEntity> service)
        {
            _service = service;
        }

        /*[HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }*/

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result.ToActionResult(this);
        }
            

        /*[HttpPost]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto)
        {
            var createdEntity = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdEntity.Id }, createdEntity);
        }*/

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
            return result.ToActionResult(this);
        }
    }
}
