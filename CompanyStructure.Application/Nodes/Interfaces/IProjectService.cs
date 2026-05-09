using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Nodes.Interfaces
{
    public interface IProjectService : INodeService<Project>
    {
        Task<ServiceResult<List<NodeResponse>>> GetAllAsync(int divisionId);
        Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int divisionId);
    }
}
