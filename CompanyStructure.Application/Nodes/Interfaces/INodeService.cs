using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface INodeService<T> where T : class, INode
    {
        Task<ServiceResult<NodeResponse?>> GetByIdAsync(int id);
        Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
