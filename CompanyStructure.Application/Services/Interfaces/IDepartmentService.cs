using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IDepartmentService : IOrganisationNodeService<Department>
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int projectId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int projectId);
    }
}
