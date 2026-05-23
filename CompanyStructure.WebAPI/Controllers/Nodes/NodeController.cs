using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Nodes
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class NodeController<TEntity> : ControllerBase
        where TEntity : class, INode
    {
        protected readonly INodeService<TEntity> _nodeService;

        protected NodeController(INodeService<TEntity> nodeService)
        {
            _nodeService = nodeService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _nodeService.GetByIdAsync(id);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateNodeRequest dto)
        {
            var result = await _nodeService.UpdateAsync(id, dto);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
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
