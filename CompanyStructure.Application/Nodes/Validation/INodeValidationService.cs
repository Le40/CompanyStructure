using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Validation
{
    public interface INodeValidationService
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
            where T : class, INode;
    }
}
