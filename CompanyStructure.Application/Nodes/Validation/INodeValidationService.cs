using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Validation
{
    public interface INodeValidationService
    {
        Task<ServiceResult<bool>> ValidateNodeAsync<T>(int? leaderId, string code, int companyId, int? excludeId = null)
            where T : class, INode;
        Task<ServiceResult<bool>> ValidateLeaderAsync<T>(int? leaderId, int companyId);
        Task<ServiceResult<bool>> ValidateCodeIsUniqueAsync<T>(
            string code,
            int companyId,
            int? excludeId = null)
            where T : class, INode;
    }
}
