using CompanyStructure.Application.Common.Extensions;
using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Employees;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Nodes.Services
{
    public class DepartmentService : NodeService<Department>, IDepartmentService
    {
        public DepartmentService(   
            AppDbContext db,
            INodeValidationService validation,
            ILogger<DepartmentService> logger)
            : base(db, validation, logger)
        {
        }

        public async Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(int projectId, PaginationQuery pagination)
        {
            var nodeExists = await _db.Projects.AnyAsync(n => n.Id == projectId);
            if (!nodeExists)
                return ServiceResult<PagedResult<NodeResponse>>.Fail(ServiceErrors.NotFound<Project>());

            var departments = await _db.Departments
                .Where(d => d.ProjectId == projectId)
                .ToPagedResultAsync<Department, NodeResponse>(pagination.Page, pagination.PageSize);

            return ServiceResult<PagedResult<NodeResponse>>.Ok(departments);
        }

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int projectId)
        {
            _logger.LogInformation("Creating department with code {Code} in project {ProjectId}", dto.Code, projectId);
            var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                _logger.LogWarning("Project with id {ProjectId} does not exist", projectId);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Project>());
            }

            var companyId = project.CompanyId;
            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<NodeResponse>.Fail(leaderValidation.Error!);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Department>(dto.Code!, companyId);

            if (!codeValidation.Success)
                return ServiceResult<NodeResponse>.Fail(codeValidation.Error!);

            var department = dto.Adapt<Department>();
            department.ProjectId = projectId;
            department.CompanyId = companyId;

            _db.Departments.Add(department);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Department with id {DepartmentId} created successfully", department.Id);
            return ServiceResult<NodeResponse>.Ok(
                department.Adapt<NodeResponse>());
        }
    }
}
