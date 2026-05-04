using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Validation
{
    public class OrganisationNodeValidationService : IOrganisationNodeValidationService
    {
        private readonly AppDbContext _db;

        public OrganisationNodeValidationService(AppDbContext db)
        {
            _db = db;
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
                return ServiceResult<bool>.Fail(
                    "Leader must be an employee of the same company.",
                    ServiceErrorType.Validation);
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
            where T : class, IOrganisationNode
        {
            var exists = await _db.Set<T>().AnyAsync(x =>
                x.CompanyId == companyId &&
                x.Code == code &&
                (!excludeId.HasValue || x.Id != excludeId.Value));

            if (exists)
            {
                return ServiceResult<bool>.Fail(
                    "Code already exists in this company.",
                    ServiceErrorType.Conflict);
            }

            return ServiceResult<bool>.Ok(true);
        }
    }
}
