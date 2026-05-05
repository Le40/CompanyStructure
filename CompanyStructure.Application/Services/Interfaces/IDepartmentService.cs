using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IDepartmentService : IOrganisationNodeService<Department>
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int projectId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int projectId);
    }
}
