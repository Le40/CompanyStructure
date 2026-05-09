using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CompanyStructure.Application.Nodes.Validation
{
    public class NodeValidationService : INodeValidationService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<NodeValidationService> _logger;

        public NodeValidationService(AppDbContext db, ILogger<NodeValidationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ServiceResult<bool>> ValidateLeaderAsync(int? leaderId, int companyId)
        {
            if (leaderId == null)
                return ServiceResult<bool>.Ok(true);

            var leaderValid = await _db.Employees.AnyAsync(e =>
                e.Id == leaderId.Value &&
                e.CompanyId == companyId);

            if (!leaderValid)
            {
                _logger.LogWarning("Validation failed for leaderId {LeaderId} in companyId {CompanyId}", leaderId, companyId);
                return ServiceResult<bool>.Fail(ServiceErrors.LeaderIsNotEmployee<Company>());
            }

            return ServiceResult<bool>.Ok(true);
        }

        /*public async Task<ServiceResult<bool>> ValidateCodeIsUniqueAsync<T>(
            string code,
            int? excludeId = null)
            where T : class, IOrganisationNode
        {
            var exists = await _db.Set<T>().AnyAsync(x =>
                x.Code == code &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
            {
                return ServiceResult<bool>.Fail(
                    "Code already exists in this company.",
                    ServiceErrorType.Conflict);
            }

            return ServiceResult<bool>.Ok(true);
        }*/

        public async Task<ServiceResult<bool>> ValidateCodeIsUniqueAsync<T>(
            string code,
            int companyId,
            int? excludeId = null)
            where T : class, INode
        {
            var exists = await _db.Set<T>().AnyAsync(x =>
                x.CompanyId == companyId &&
                x.Code == code &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
            {
                _logger.LogWarning("Validation failed for code {Code} in companyId {CompanyId}", code, companyId);
                return ServiceResult<bool>.Fail(ServiceErrors.DuplicateCode<T>());
            }

            return ServiceResult<bool>.Ok(true);
        }
    }
}
