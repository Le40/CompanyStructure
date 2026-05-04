using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IOrganisationNodeService<T> where T : class, IOrganisationNode
    {
        //Task<List<GetOrganisationNodeDTO>> GetAllAsync(int? parentId = null);
        Task<ServiceResult<GetOrganisationNodeDTO?>> GetByIdAsync(int id);
        //Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int? parentId = null);
        Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto);
        Task<ServiceResult<bool>> DeleteAsync(int id);
    }
}
