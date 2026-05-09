using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IOrganisationNodeService<T> where T : class, IOrganisationNode
    {
        Task<ServiceResult<GetOrganisationNodeDTO?>> GetByIdAsync(int id);
        Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
