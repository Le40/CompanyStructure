using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces.Validation
{
    public interface INodeValidationService
    {
        Task<ServiceResult> ValidateNodeAsync<T>(int? leaderId, string code, int companyId, int? excludeId = null)
            where T : class, INode;
        Task<ServiceResult> ValidateLeaderAsync<T>(int? leaderId, int companyId);
        Task<ServiceResult> ValidateCodeIsUniqueAsync<T>(
            string code,
            int companyId,
            int? excludeId = null)
            where T : class, INode;
    }
}
