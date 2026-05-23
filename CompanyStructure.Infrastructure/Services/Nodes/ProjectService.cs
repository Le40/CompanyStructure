using CompanyStructure.Infrastructure.Extensions;
using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Interfaces.Validation;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Infrastructure.Services.Nodes
{
    public class ProjectService : NodeService<Project>, IProjectService
    {
        public ProjectService(
            AppDbContext db,
            INodeValidationService validation,
            ILogger<ProjectService> logger)
            : base(db, validation, logger)
        {
        }

        public async Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(int divisionId, PaginationQuery pagination)
        {
            var nodeExists = await _db.Divisions.AnyAsync(n => n.Id == divisionId);
            if (!nodeExists)
                return ServiceResult<PagedResult<NodeResponse>>.Fail(ServiceErrors.NotFound<Division>(divisionId));

            var projects = await _db.Projects
                .Where(d => d.DivisionId == divisionId)
                .ToPagedResultAsync<Project, NodeResponse>(pagination.Page, pagination.PageSize);

            return ServiceResult<PagedResult<NodeResponse>>.Ok(projects);
        }

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int divisionId)
        {
            _logger.LogInformation("Creating project with name {Name} and code {Code} under division {DivisionId}", dto.Name, dto.Code, divisionId);
            var division = await _db.Divisions.FirstOrDefaultAsync(d => d.Id == divisionId);

            if (division == null)
            {
                _logger.LogWarning("Division with id {DivisionId} does not exist.", divisionId);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Division>(divisionId));
            }

            var validation = await _validation.ValidateNodeAsync<Project>(dto.LeaderId, dto.Code, division.CompanyId);
            if (!validation.Success)
                return ServiceResult<NodeResponse>.Fail(validation.Error!);

            var project = dto.Adapt<Project>();
            project.DivisionId = division.Id;
            project.CompanyId = division.CompanyId;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} created successfully", project.Id);
            return ServiceResult<NodeResponse>.Ok(
                project.Adapt<NodeResponse>());
        }
    }
}
