using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface IDepartmentService : INodeService<Department>
    {
        Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(int projectId, PaginationQuery pagination);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int projectId);
    }
}
