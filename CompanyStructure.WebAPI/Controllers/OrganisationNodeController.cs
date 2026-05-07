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
        protected readonly IOrganisationNodeService<TEntity> _nodeService;

        protected OrganisationNodeController(IOrganisationNodeService<TEntity> nodeService)
        {
            _nodeService = nodeService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nodeService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOrganisationNodeDTO dto)
        {
            var result = await _nodeService.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _nodeService.DeleteAsync(id);
            if (!result.Success)
                return result.ToActionResult(this);

            return NoContent();
        }
    }
}
