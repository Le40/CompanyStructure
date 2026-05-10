using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface IDivisionService : INodeService<Division>
    {
        Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(int companyId, PaginationQuery pagination);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int companyId);
    }
}
