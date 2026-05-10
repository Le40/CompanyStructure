using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface IProjectService : INodeService<Project>
    {
        Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(int divisionId, PaginationQuery pagination);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int divisionId);
    }
}
