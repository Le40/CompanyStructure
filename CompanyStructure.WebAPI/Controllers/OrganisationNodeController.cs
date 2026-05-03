using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class OrganisationNodeController<TEntity> : ControllerBase
        where TEntity : class, IOrganizationNode
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
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
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
            var updatedEntity = await _service.UpdateAsync(id, dto);
            if (!updatedEntity.Success)
            {
                return NotFound();
            }
            return Ok(updatedEntity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
