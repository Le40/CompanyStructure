using CompanyStructure.Application.DTOs.OrganisationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync();
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto);
        Task<ServiceResult<GetOrganisationNodeDTO?>> GetByIdAsync(int id);
        Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
