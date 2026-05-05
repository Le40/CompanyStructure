using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;

namespace CompanyStructure.Application.Services
{
    // Base service for organisation nodes (Division, Project, Department), contains common methods for getting by id, updating and deleting nodes.
    public class OrganisationNodeService<T> : IOrganisationNodeService<T> 
        where T : class, IOrganisationNode
    {
        protected readonly AppDbContext _db;
        protected readonly IOrganisationNodeValidationService _validation;

        public OrganisationNodeService(AppDbContext db, IOrganisationNodeValidationService validation)
        {
            _db = db;
            _validation = validation;
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO?>> GetByIdAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
            {
                return ServiceResult<GetOrganisationNodeDTO?>.Fail("Node not found", ServiceErrorType.NotFound);
            }
            return ServiceResult<GetOrganisationNodeDTO?>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto)
        {
            var node = await _db.Set<T>().FindAsync(id);

            if (node == null)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Node not found.",
                    ServiceErrorType.NotFound);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<T>(
                dto.Code!,
                node.CompanyId,
                excludeId: id);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    codeValidation.Error!,
                    codeValidation.ErrorType!.Value);

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, node.CompanyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);

            var updatedNode = dto.Adapt(node);
            _db.Entry(node).CurrentValues.SetValues(updatedNode);
            await _db.SaveChangesAsync();
            return ServiceResult<GetOrganisationNodeDTO>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
                return ServiceResult<bool>.Fail("Node was not found", ServiceErrorType.NotFound);

            _db.Set<T>().Remove(node);
            await _db.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
    }
}
