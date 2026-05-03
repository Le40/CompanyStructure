using CompanyStructure.Application.DTOs.OrganisationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IProjectService
    {
        Task<List<GetOrganisationNodeDTO>> GetAllAsync(int divisionId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int divisionId);
    }
}
