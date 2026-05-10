using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(PaginationQuery pagination);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto);
        Task<ServiceResult<NodeResponse?>> GetByIdAsync(int id);
        Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
