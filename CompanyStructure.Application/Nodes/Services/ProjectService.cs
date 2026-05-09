using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Validation;
using CompanyStructure.Application.Results;
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

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int divisionId)
        {
            _logger.LogInformation("Creating project with name {Name} and code {Code} under division {DivisionId}", dto.Name, dto.Code, divisionId);
            var divisionExists = await _db.Divisions.AnyAsync(d => d.Id == divisionId);

            if (!divisionExists)
            {
                _logger.LogWarning("Division with id {DivisionId} does not exist", divisionId);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Division>());
            }

            var companyId = await _db.Divisions
                .Where(d => d.Id == divisionId)
                .Select(d => d.CompanyId)
                .FirstOrDefaultAsync();

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<NodeResponse>.Fail(leaderValidation.Error!);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Project>(dto.Code!, companyId);

            if (!codeValidation.Success)
                return ServiceResult<NodeResponse>.Fail(codeValidation.Error!);

            var project = dto.Adapt<Project>();
            project.DivisionId = divisionId;
            project.CompanyId = companyId;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} created successfully", project.Id);
            return ServiceResult<NodeResponse>.Ok(
                project.Adapt<NodeResponse>());
        }

        public async Task<ServiceResult<List<NodeResponse>>> GetAllAsync(int divisionId)
        {
            var nodeExists = await _db.Divisions.AnyAsync(n => n.Id == divisionId);
            if (!nodeExists)  
                return ServiceResult<List<NodeResponse>>.Fail(ServiceErrors.NotFound<Division>());

            var projects = await _db.Projects.Where(d => d.DivisionId == divisionId).ToListAsync();
            return ServiceResult<List<NodeResponse>>.Ok(projects.Adapt<List<NodeResponse>>());
        }
    }
}
