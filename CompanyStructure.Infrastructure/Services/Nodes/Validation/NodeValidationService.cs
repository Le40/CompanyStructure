using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.Interfaces.Validation;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace CompanyStructure.Infrastructure.Services.Nodes.Validation
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

        public async Task<ServiceResult<bool>> ValidateNodeAsync<T>(int? leaderId, string code, int companyId, int? excludeId = null)
            where T : class, INode
        {
            var leaderValidation = await ValidateLeaderAsync<T>(leaderId, companyId);
            if (!leaderValidation.Success)
                return leaderValidation;

            var codeValidation = await ValidateCodeIsUniqueAsync<T>(code, companyId, excludeId);
            if (!codeValidation.Success)
                return codeValidation;

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ValidateLeaderAsync<T>(int? leaderId, int companyId)
        {
            if (leaderId == null)
                return ServiceResult<bool>.Ok(true);

            var leaderValid = await _db.Employees.AnyAsync(e =>
                e.Id == leaderId.Value &&
                e.CompanyId == companyId);

            if (!leaderValid)
            {
                _logger.LogWarning("Leader with id {LeaderId} does not exist in companyId {CompanyId}", leaderId, companyId);
                return ServiceResult<bool>.Fail(ServiceErrors.InvalidLeader<T>());
            }

            return ServiceResult<bool>.Ok(true);
        }

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
                _logger.LogWarning("Code for {Type} in company with id {CompanyId} already existst.", typeof(T).Name, companyId);
                return ServiceResult<bool>.Fail(ServiceErrors.DuplicateCode<T>());
            }

            return ServiceResult<bool>.Ok(true);
        }
    }
}
