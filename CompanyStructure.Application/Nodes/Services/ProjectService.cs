using CompanyStructure.Application.Common.Extensions;
using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Nodes.Services
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
                return ServiceResult<PagedResult<NodeResponse>>.Fail(ServiceErrors.NotFound<Division>());

            var projects = await _db.Projects
                .Where(d => d.DivisionId == divisionId)
                .ToPagedResultAsync<Project, NodeResponse>(pagination.Page, pagination.PageSize);

            return ServiceResult<PagedResult<NodeResponse>>.Ok(projects);
        }

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int divisionId)
        {
            _logger.LogInformation("Creating project with name {Name} and code {Code} under division {DivisionId}", dto.Name, dto.Code, divisionId);
            var context = await GetProjectCreateContextAsync(divisionId);
            if (!context.Success)
                return ServiceResult<NodeResponse>.Fail(context.Error!);

            var createContext = context.Data!;

            var validation = await _validation.ValidateNodeAsync<Project>(dto.LeaderId, dto.Code, createContext.CompanyId);
            if (!validation.Success)
                return ServiceResult<NodeResponse>.Fail(validation.Error!);

            var project = dto.Adapt<Project>();
            project.DivisionId = createContext.DivisionId;
            project.CompanyId = createContext.CompanyId;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} created successfully", project.Id);
            return ServiceResult<NodeResponse>.Ok(
                project.Adapt<NodeResponse>());
        }

        private record ProjectCreateContext(int DivisionId, int CompanyId);


        private async Task<ServiceResult<ProjectCreateContext>> GetProjectCreateContextAsync(int divisionId)
        {
            var division = await _db.Divisions.FirstOrDefaultAsync(d => d.Id == divisionId);

            if (division == null)
            {
                _logger.LogWarning("Division with id {DivisionId} does not exist.", divisionId);
                return ServiceResult<ProjectCreateContext>.Fail(ServiceErrors.NotFound<Division>());
            }

            return ServiceResult<ProjectCreateContext>.Ok(new ProjectCreateContext(divisionId, division.CompanyId));
        }
    }
}
