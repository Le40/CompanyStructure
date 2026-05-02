using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services
{
    public interface IOrganisationNodeService<T> where T : class, IOrganizationNode
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T node);
        Task<T?> UpdateAsync(int id, T updatedNode);
        Task DeleteAsync(int id);
    }
}
