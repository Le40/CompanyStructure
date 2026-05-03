using CompanyStructure.Application.DTOs.OrganisationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<List<GetOrganisationNodeDTO>> GetAllAsync();
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto);
    }
}
