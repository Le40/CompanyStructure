using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Services
{
    public class ProjectService : OrganisationNodeService<Project>, IProjectService
    {
        public ProjectService(
            AppDbContext db,
            IOrganisationNodeValidationService validation,
            ILogger<ProjectService> logger)
            : base(db, validation, logger)
        {
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int divisionId)
        {
            _logger.LogInformation("Creating project with name {Name} and code {Code} under division {DivisionId}", dto.Name, dto.Code, divisionId);
            var divisionExists = await _db.Divisions.AnyAsync(d => d.Id == divisionId);

            if (!divisionExists)
            {
                _logger.LogWarning("Division with id {DivisionId} does not exist", divisionId);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(ServiceErrors.NotFound<Division>());
            }

            var companyId = await _db.Divisions
                .Where(d => d.Id == divisionId)
                .Select(d => d.CompanyId)
                .FirstOrDefaultAsync();

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(leaderValidation.Error!);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Project>(dto.Code!, companyId);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(codeValidation.Error!);

            var project = dto.Adapt<Project>();
            project.DivisionId = divisionId;
            project.CompanyId = companyId;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Project with id {ProjectId} created successfully", project.Id);
            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                project.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int divisionId)
        {
            var nodeExists = await _db.Divisions.AnyAsync(n => n.Id == divisionId);
            if (!nodeExists)  
                return ServiceResult<List<GetOrganisationNodeDTO>>.Fail(ServiceErrors.NotFound<Division>());

            var projects = await _db.Projects.Where(d => d.DivisionId == divisionId).ToListAsync();
            return ServiceResult<List<GetOrganisationNodeDTO>>.Ok(projects.Adapt<List<GetOrganisationNodeDTO>>());
        }
    }
}
