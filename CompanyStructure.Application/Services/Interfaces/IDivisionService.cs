using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IDivisionService : IOrganisationNodeService<Division>
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int companyId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int companyId);
    }
}
