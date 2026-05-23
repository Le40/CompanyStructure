using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Interfaces.Validation;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Infrastructure.Services.Nodes
{
    // Base service for organisation nodes (Division, Project, Department), contains common methods for getting by id, updating and deleting nodes.
    public class NodeService<T> : INodeService<T> 
        where T : class, INode
    {
        protected readonly AppDbContext _db;
        protected readonly INodeValidationService _validation;
        protected readonly ILogger<NodeService<T>> _logger;
        protected readonly string _nodeTypeName = typeof(T).Name;

        public NodeService(AppDbContext db, INodeValidationService validation, ILogger<NodeService<T>> logger)
        {
            _db = db;
            _validation = validation;
            _logger = logger;
        }

        public async Task<ServiceResult<NodeResponse?>> GetByIdAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
            {
                return ServiceResult<NodeResponse?>.Fail(ServiceErrors.NotFound<T>(id));
            }
            return ServiceResult<NodeResponse?>.Ok(node.Adapt<NodeResponse>());
        }

        public async Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto)
        {
            var node = await _db.Set<T>().FindAsync(id);

            if (node == null)
            {
                _logger.LogWarning("{NodeType} with id {NodeId} not found for update", _nodeTypeName, id);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<T>(id));
            }

            var validation = await _validation.ValidateNodeAsync<T>(dto.LeaderId, dto.Code, node.CompanyId, excludeId: node.Id);
            if (!validation.Success)
                return ServiceResult<NodeResponse>.Fail(validation.Error!);

            dto.Adapt(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("{NodeType} with id {NodeId} updated successfully", _nodeTypeName, id);
            return ServiceResult<NodeResponse>.Ok(node.Adapt<NodeResponse>());
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
            {
                _logger.LogWarning("{NodeType} with id {NodeId} not found for deletion", _nodeTypeName, id);
                return ServiceResult.Fail(ServiceErrors.NotFound<T>(id));
            }
            _db.Set<T>().Remove(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("{NodeType} with id {NodeId} deleted successfully", _nodeTypeName, id);
            return ServiceResult.Ok();
        }
    }
}
