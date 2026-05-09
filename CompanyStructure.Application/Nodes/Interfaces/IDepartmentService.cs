using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface IDepartmentService : INodeService<Department>
    {
        Task<ServiceResult<List<NodeResponse>>> GetAllAsync(int projectId);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int projectId);
    }
}
