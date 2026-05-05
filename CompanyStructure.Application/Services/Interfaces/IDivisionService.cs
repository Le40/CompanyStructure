using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IDivisionService : IOrganisationNodeService<Division>
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int companyId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int companyId);
    }
}
