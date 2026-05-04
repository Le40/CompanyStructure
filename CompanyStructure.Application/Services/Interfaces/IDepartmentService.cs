using CompanyStructure.Application.DTOs.OrganisationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int projectId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int projectId);
    }
}
