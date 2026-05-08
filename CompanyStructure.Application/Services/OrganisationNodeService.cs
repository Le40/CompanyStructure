using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Services
{
    // Base service for organisation nodes (Division, Project, Department), contains common methods for getting by id, updating and deleting nodes.
    public class OrganisationNodeService<T> : IOrganisationNodeService<T> 
        where T : class, IOrganisationNode
    {
        protected readonly AppDbContext _db;
        protected readonly IOrganisationNodeValidationService _validation;
        protected readonly ILogger<OrganisationNodeService<T>> _logger;
        protected readonly string _nodeTypeName = typeof(T).Name;

        public OrganisationNodeService(AppDbContext db, IOrganisationNodeValidationService validation, ILogger<OrganisationNodeService<T>> logger)
        {
            _db = db;
            _validation = validation;
            _logger = logger;
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO?>> GetByIdAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
            {
                return ServiceResult<GetOrganisationNodeDTO?>.Fail(ServiceErrors.NotFound<T>());
            }
            return ServiceResult<GetOrganisationNodeDTO?>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto)
        {
            var node = await _db.Set<T>().FindAsync(id);

            if (node == null)
            {
                _logger.LogWarning("{NodeType} with id {NodeId} not found for update", _nodeTypeName, id);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(ServiceErrors.NotFound<T>());
            }
            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<T>(
                dto.Code!,
                node.CompanyId,
                excludeId: id);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(codeValidation.Error!);

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, node.CompanyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(leaderValidation.Error!);

            dto.Adapt(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("{NodeType} with id {NodeId} updated successfully", _nodeTypeName, id);
            return ServiceResult<GetOrganisationNodeDTO>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
            {
                _logger.LogWarning("{NodeType} with id {NodeId} not found for deletion", _nodeTypeName, id);
                return ServiceResult<bool>.Fail(ServiceErrors.NotFound<T>());
            }
            _db.Set<T>().Remove(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("{NodeType} with id {NodeId} deleted successfully", _nodeTypeName, id);
            return ServiceResult<bool>.Ok(true);
        }
    }
}
