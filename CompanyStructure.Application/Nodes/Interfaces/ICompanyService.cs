using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.DTOs;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(PaginationQuery pagination);
        Task<ServiceResult<NodeResponse?>> GetByIdAsync(int id);
        Task<ServiceResult<CompanyStructureResponse>> GetStructureByIdAsync(int id);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateCompanyRequest dto);
        Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
