using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface IDivisionService : INodeService<Division>
    {
        Task<ServiceResult<List<NodeResponse>>> GetAllAsync(int companyId);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int companyId);
    }
}
