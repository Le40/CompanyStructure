using CompanyStructure.Application.Results;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult<List<NodeResponse>>> GetAllAsync();
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto);
        Task<ServiceResult<NodeResponse?>> GetByIdAsync(int id);
        Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
