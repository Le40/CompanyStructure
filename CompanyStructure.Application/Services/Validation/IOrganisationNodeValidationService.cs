using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Validation
{
    public interface IOrganisationNodeValidationService
    {
        Task<ServiceResult<bool>> ValidateLeaderAsync(int? leaderId, int companyId);

        /*Task<ServiceResult<bool>> ValidateCodeIsUniqueAsync<T>(
            string code,
            int? excludeId = null)
            where T : class, IOrganisationNode;*/

        Task<ServiceResult<bool>> ValidateCodeIsUniqueAsync<T>(
            string code,
            int companyId,
            int? excludeId = null)
            where T : class, IOrganisationNode;
    }
}
