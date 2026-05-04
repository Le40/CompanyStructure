using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace CompanyStructure.Application.Services
{
    public class OrganisationNodeService<T> : IOrganisationNodeService<T> 
        where T : class, IOrganisationNode
    {
        protected readonly AppDbContext _db;
        protected readonly IOrganisationNodeValidationService _validation;

        public OrganisationNodeService(AppDbContext db, IOrganisationNodeValidationService validation)
        {
            _db = db;
            _validation = validation;
        }

        /*public async Task<List<GetOrganisationNodeDTO>> GetAllAsync(int? parentId = null)
        {
            var query = _db.Set<T>().AsQueryable();

            if (typeof(T) == typeof(Division) && parentId != null)
            {
                query = query.Where(x => ((Division)(object)x).CompanyId == parentId);
            }
            else if (typeof(T) == typeof(Project) && parentId != null)
            {
                query = query.Where(x => ((Project)(object)x).DivisionId == parentId);
            }
            else if (typeof(T) == typeof(Department) && parentId != null)
            {
                query = query.Where(x => ((Department)(object)x).ProjectId == parentId);
            }

            var entities = await query.ToListAsync();

            return entities.Adapt<List<GetOrganisationNodeDTO>>();
        }*/

        public async Task<GetOrganisationNodeDTO?> GetByIdAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
            {
                return null;
            }
            return node.Adapt<GetOrganisationNodeDTO>();
        }

        /*public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync( CreateOrganisationNodeDTO dto, int? parentId = null)
        {
            var node = dto.Adapt<T>();

            var parentValidation = await ValidateAndSetParentNodeAsync(node, parentId);
            if (!parentValidation.Success)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    parentValidation.Error!,
                    parentValidation.ErrorType ?? ServiceErrorType.Validation
                );
            }


            _db.Set<T>().Add(node);
            await _db.SaveChangesAsync();

            return ServiceResult<GetOrganisationNodeDTO>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }*/

        public async Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto)
        {
            var node = await _db.Set<T>().FindAsync(id);

            if (node == null)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Node not found.",
                    ServiceErrorType.NotFound);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<T>(
                dto.Code!,
                node.CompanyId,
                excludeId: id);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    codeValidation.Error!,
                    codeValidation.ErrorType!.Value);

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, node.CompanyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);

            var updatedNode = dto.Adapt(node);
            _db.Entry(node).CurrentValues.SetValues(updatedNode);
            await _db.SaveChangesAsync();
            return ServiceResult<GetOrganisationNodeDTO>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
                return ServiceResult<bool>.Fail("Node was not found", ServiceErrorType.NotFound);

            _db.Set<T>().Remove(node);
            await _db.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidateAndSetParentNodeAsync(T node, int? parentId)
        {
            if (node is Company)
            {
                if (parentId != null)
                {
                    return ServiceResult<bool>.Fail("Company cannot have a parent node", ServiceErrorType.Validation);
                }
                return ServiceResult<bool>.Ok(true);
            }

            if (parentId == null)
            {
                return ServiceResult<bool>.Fail("Parent node ID is required", ServiceErrorType.Validation);
            }

            if (node is Division division)
            {
                var companyExists = await _db.Companies.AnyAsync(c => c.Id == parentId.Value);
                if (!companyExists)
                {
                    return ServiceResult<bool>.Fail("Parent company not found", ServiceErrorType.NotFound);
                }

                division.CompanyId = parentId.Value;
                return ServiceResult<bool>.Ok(true);
            }

            if (node is Project project)
            {
                var divisionExists = await _db.Divisions.AnyAsync(d => d.Id == parentId.Value);
                if (!divisionExists)
                {
                    return ServiceResult<bool>.Fail("Parent division not found", ServiceErrorType.NotFound);
                }

                project.DivisionId = parentId.Value;
                return ServiceResult<bool>.Ok(true);
            }

            if (node is Department department)
            {
                var projectExists = await _db.Projects.AnyAsync(p => p.Id == parentId.Value);
                if (!projectExists)
                {
                    return ServiceResult<bool>.Fail("Parent project not found", ServiceErrorType.NotFound);
                }

                department.ProjectId = parentId.Value;
                return ServiceResult<bool>.Ok(true);
            }

            return ServiceResult<bool>.Fail("Unsupported node type", ServiceErrorType.Validation);

        }
    }
}
