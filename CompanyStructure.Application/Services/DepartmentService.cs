using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _db;

        public DepartmentService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int projectId)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);

            if (!projectExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Project does not exist.",
                    ServiceErrorType.NotFound);
            }

            var companyId = await _db.Projects
                .Where(p => p.Id == projectId)
                .Select(p => p.CompanyId)
                .FirstOrDefaultAsync();

            var codeExists = await _db.Departments
                .AnyAsync(d => d.CompanyId == companyId && d.Code == dto.Code);

            if (codeExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Division with this code already exists in this company.",
                    ServiceErrorType.Conflict);
            }

            var companyId = await _db.Projects
                .Where(p => p.Id == projectId)
                .Select(p => p.CompanyId)
                .FirstOrDefaultAsync();

            if (dto.LeaderId != null)
            {
                var leaderValid = await _db.Employees
                    .AnyAsync(e =>
                        e.Id == dto.LeaderId.Value &&
                        e.CompanyId == companyId);

                if (!leaderValid)
                {
                    return ServiceResult<GetOrganisationNodeDTO>.Fail(
                        "Leader must be an employee of the same company.",
                        ServiceErrorType.Validation);
                }
            }

            var department = dto.Adapt<Department>();
            department.ProjectId = projectId;

            _db.Departments.Add(department);
            await _db.SaveChangesAsync();

            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                department.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<List<GetOrganisationNodeDTO>> GetAllAsync(int projectId)
        {
            var departments = await _db.Departments.Where(d => d.ProjectId == projectId).ToListAsync();
            return departments.Adapt<List<GetOrganisationNodeDTO>>();
        }
    }
}
