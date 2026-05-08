using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IProjectService : IOrganisationNodeService<Project>
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int divisionId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int divisionId);
    }
}
