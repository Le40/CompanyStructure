using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IProjectService : IOrganisationNodeService<Project>
    {
        Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int divisionId);
        Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int divisionId);
    }
}
